﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.AuthModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.UserModel;
using WMK_BE_BusinessLogic.Helpers;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class AuthService : IAuthService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IConfiguration _configuration;
		private readonly IMapper _mapper;
		#region Validator
		private readonly LoginModelValidator _loginValidator;
		private readonly RegisterModelValidator _registerValidator;
		private readonly ResetPasswordModelValidator _resetPassValidator;
		private readonly ResetPasswordByEmailModelValidator _resetPassByEmailValidator;
		private readonly SendCodeByEmailModelValidator _sendCodeValidator;
		#endregion
		public AuthService(IUnitOfWork unitOfWork , IConfiguration configuration , IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_configuration = configuration;
			_mapper = mapper;

			#region Validation construtor
			_loginValidator = new LoginModelValidator();
			_registerValidator = new RegisterModelValidator();
			_resetPassValidator = new ResetPasswordModelValidator();
			_resetPassByEmailValidator = new ResetPasswordByEmailModelValidator();
			_sendCodeValidator = new SendCodeByEmailModelValidator();
			#endregion
		}

		#region Login
		public async Task<ResponseObject<UserResponse>> LoginAsync(LoginModel model)
		{
			var result = new ResponseObject<UserResponse>();
			var validationResult = _loginValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			// Check userExist by email or username
			var userExist = await _unitOfWork.UserRepository.GetByEmailOrUserNameAsync(model.EmailOrUserName);
			if ( userExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Email, tên người dùng hoặc mật khẩu không chính xác!";
				return result;
			}
			if ( userExist != null )
			{
				//check Login
				var resultLogin = HashHelper.VerifyHash(model.Password , userExist.PasswordHash);
				if ( resultLogin )
				{
					//if addmin login
					switch ( userExist.Role )
					{
						case WMK_BE_RecipesAndPlans_DataAccess.Enums.Role.Customer:
							//check email confirm
							if ( userExist.EmailConfirm == WMK_BE_RecipesAndPlans_DataAccess.Enums.EmailConfirm.NotConfirm )
							{
								//create code then send mail
								var code = GenerateRandomCode(6);
								userExist.Code = code;
								var updateResult = await _unitOfWork.UserRepository.UpdateAsync(userExist);
								if ( updateResult )
								{
									await _unitOfWork.CompleteAsync();
									result.StatusCode = 405;
									result.Message = code;
									result.Data = _mapper.Map<UserResponse>(userExist);
									return result;
								}
								result.StatusCode = 500;
								result.Message = "Update code to DB faild!";
								return result;
							}
							//check account lock
							if ( userExist.Status == WMK_BE_RecipesAndPlans_DataAccess.Enums.BaseStatus.UnAvailable )
							{
								result.StatusCode = 423;
								result.Message = "Tài khoản của bạn hiện đang bị khóa. Vui lòng liên hệ bộ phận chăm sóc khách hàng để được hỗ trợ!";//Account locked! Please contact administrator!
                                return result;
							}
							break;
						default:
							break;
					}

					//Success -> Reset AccessFailedCount
					userExist.AccessFailedCount = 0;
					await _unitOfWork.UserRepository.UpdateAsync(userExist);
					await _unitOfWork.CompleteAsync();
					//Token
					var tokenString = GenerateToken(userExist);
					if ( tokenString != null )
					{
						result.StatusCode = 200;
						result.Message = tokenString;
						result.Data = _mapper.Map<UserResponse>(userExist);
						return result;
					}
					else
					{
						result.StatusCode = 400;
						result.Message = "Create token fail!";
						return result;
					}
				}
				else
				{
					//Login fail -> increase AccessFailedCount
					userExist.AccessFailedCount++;
					//check user login fail more than 5 time
					if ( userExist.AccessFailedCount >= 5 )
					{
						//temporary account lock
						userExist.Status = WMK_BE_RecipesAndPlans_DataAccess.Enums.BaseStatus.UnAvailable;
						await _unitOfWork.UserRepository.UpdateAsync(userExist);
						await _unitOfWork.CompleteAsync();
						result.StatusCode = 423;
						result.Message = "Sai thông tin đăng nhập quá 5 lần. Tài khoàn của bạn bị khóa!";//Wrong more than 5 time then lock account!
                        return result;
					}
					else
					{
						await _unitOfWork.UserRepository.UpdateAsync(userExist);
						await _unitOfWork.CompleteAsync();
						result.StatusCode = 401;
						result.Message = "Email, tên người dùng hoặc mật khẩu không chính xác!";
						return result;
					}
				}
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Email, tên người dùng hoặc mật khẩu không chính xác!";//User not found!
                return result;
			}
		}

		public string GenerateToken(User user)
		{
			// Tuần tự hóa đối tượng User thành chuỗi JSON
			var userJson = JsonConvert.SerializeObject(user);
			//create list claim
			var authClaims = new List<Claim>
				{
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					//new Claim(ClaimTypes.Name, user.UserName.ToString()),
					//new Claim(ClaimTypes.Email, user.Email.ToString()),
					new Claim(ClaimTypes.Role, user.Role.ToString()),
					new Claim("User", userJson),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
				};
			//check and add email into list Claim
			var signingKey = _configuration["JWT:IssuerSigningKey"];
			if ( !string.IsNullOrEmpty(signingKey) )
			{
				//create key
				var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
				//create JWT
				var token = new JwtSecurityToken(
					issuer: _configuration["JWT:ValidIssuer"] ,
					audience: _configuration["JWT:ValidAudience"] ,
					claims: authClaims ,
					expires: DateTime.UtcNow.AddHours(7).AddDays(7),
					signingCredentials: new SigningCredentials(authKey , SecurityAlgorithms.HmacSha512Signature)
				);

				//string JWT
				return new JwtSecurityTokenHandler().WriteToken(token).ToString();
			}
			return "";
		}
		#endregion

		#region Confirm mail
		public async Task<ResponseObject<BaseUserResponse>> CheckEmailConfirmAsync(CheckEmailConfirmRequest model)
		{
			var result = new ResponseObject<BaseUserResponse>();
			var userExist = await _unitOfWork.UserRepository.GetByEmailOrUserNameAsync(model.Email);
			if ( userExist != null )
			{
				if ( userExist.Code == model.Code && !string.IsNullOrEmpty(userExist.Code) )
				{
					userExist.EmailConfirm = WMK_BE_RecipesAndPlans_DataAccess.Enums.EmailConfirm.Confirm;
					userExist.Code = "";
					var updateResult = await _unitOfWork.UserRepository.UpdateAsync(userExist);
					if ( updateResult )
					{
						await _unitOfWork.CompleteAsync();
						result.StatusCode = 200;
						result.Message = "Email xác thực thành công";
						return result;
					}
					else
					{
						result.StatusCode = 500;
						result.Message = "Email xác thực không thành công";
						return result;
					}
				}
				else
				{
					result.StatusCode = 403;
					result.Message = "Mã xác thực không đúng!";
					return result;
				}
			}
			result.StatusCode = 404;
			result.Message = "Người dùng không tồn tại!";
			return result;
		}
		#endregion

		#region Register
		public async Task<ResponseObject<UserResponse>> RegisterEmailAsync(RegisterModel model)
		{
			var result = new ResponseObject<UserResponse>();
			var validationResult = _registerValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			//check confirm password
			if ( !string.Equals(model.Password , model.ConfirmPassword) )
			{
				result.StatusCode = 400;
				result.Message = "Mật khẩu và Mật khẩu xác nhận không khớp nhau!";
				return result;
			}
			//check user exists
			var userExists = await _unitOfWork.UserRepository.GetByEmailOrUserNameAsync(model.Email);
			if ( userExists != null )
			{
				result.StatusCode = 403;
				result.Message = "Email này đã được sử dụng!";
				return result;
			}
			//create user
			var newUser = new User()
			{
				Email = model.Email ,
				UserName = model.Email ,
				FirstName = model.FirstName ,
				LastName = model.LastName ,
				Phone = model.Phone ,
				Gender = model.Gender ,
				EmailConfirm = WMK_BE_RecipesAndPlans_DataAccess.Enums.EmailConfirm.NotConfirm
			};
			//check create user
			var resultUser = await _unitOfWork.UserRepository.CreateAsync(newUser);
			if ( resultUser )
			{
				await _unitOfWork.CompleteAsync();
				//add password, set Role
				newUser.Role = WMK_BE_RecipesAndPlans_DataAccess.Enums.Role.Customer;
				newUser.PasswordHash = HashHelper.GetSignature256(model.Password);
				var code = GenerateRandomCode(6);
				newUser.Code = code;
				await _unitOfWork.UserRepository.UpdateAsync(newUser);
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = code;
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Tạo người dùng không thành công!";
				return result;
			}
		}

		#endregion

		#region Reset password
		public async Task<ResponseObject<BaseUserResponse>> ResetPasswordAsync(ResetPasswordRequest model)
		{
			var result = new ResponseObject<BaseUserResponse>();
			var validationResult = _resetPassValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			//check userExist
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(model.Id);
			if ( userExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Người dùng không tồn tại!";
				return result;
			}
			//check old pass 
			if ( !HashHelper.VerifyHash(model.OldPassword , userExist.PasswordHash) )
			{
				result.StatusCode = 403;
				result.Message = "Mật khẩu cũ không khớp!";
				return result;
			}
			//update new password
			//check confirmPassword
			if ( !string.Equals(model.NewPassword , model.ConfirmPassword) )
			{
				result.StatusCode = 403;
				result.Message = "Mật khẩu mới và mật khẩu xác nhận không khớp nhau!";
				return result;
			}
			userExist.PasswordHash = HashHelper.GetSignature256(model.NewPassword);
			var updateResult = await _unitOfWork.UserRepository.UpdateAsync(userExist);
			if ( updateResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Đổi mật khẩu thành công.";
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Đổi mật khẩu không thành công!";
				return result;
			}
		}

		public async Task<ResponseObject<BaseUserResponse>> ResetPasswordEmailAsync(ResetPasswordByEmailRequest model)
		{
			var result = new ResponseObject<BaseUserResponse>();
			var validationResult = _resetPassByEmailValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			//check email exist and email confirm
			var userExist = await _unitOfWork.UserRepository.GetByEmailOrUserNameAsync(model.Email);
			if ( userExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Người dùng không tồn tại!";
				return result;
			}
			if ( userExist.EmailConfirm == WMK_BE_RecipesAndPlans_DataAccess.Enums.EmailConfirm.NotConfirm )
			{
				result.StatusCode = 405;
				result.Message = "Email chưa được xác nhận!";
				return result;
			}
			//check code reset
			if ( userExist.Code != null && userExist.Code.Equals(model.CodeReset) )
			{
				//check confirmPassword
				if ( !string.Equals(model.NewPassword , model.ConfirmPassword) )
				{
					result.StatusCode = 403;
					result.Message = "Mật khẩu mới và mật khẩu xác nhận không khớp nhau!";
					return result;
				}
				//reset pass if code match
				userExist.Code = "";
				userExist.PasswordHash = HashHelper.GetSignature256(model.NewPassword);
				var updateResult = await _unitOfWork.UserRepository.UpdateAsync(userExist);
				if ( updateResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Đổi mật khẩu thành công.";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Đổi mật khẩu không thành công";
					return result;
				}
			}
			result.StatusCode = 403;
			result.Message = "Sai mã xác thực";
			return result;
		}

		public async Task<ResponseObject<BaseUserResponse>> SendCodeResetEmailAsync(SendCodeByEmailRequest model)
		{
			var result = new ResponseObject<BaseUserResponse>();
			var validationResult = _sendCodeValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			var userExist = await _unitOfWork.UserRepository.GetByEmailOrUserNameAsync(model.Email);
			if ( userExist == null )
			{
				result.StatusCode = 403;
				result.Message = "Người dùng không tồn tại!";
				return result;
			}
			//send Code and save to database
			var codeReset = GenerateRandomCode(6);
			userExist.Code = codeReset;
			var updateResult = await _unitOfWork.UserRepository.UpdateAsync(userExist);
			if ( updateResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = codeReset;
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Cập nhật verify code không thành công!";//Failed update code reset!
                return result;
			}
		}

		#endregion

		private string GenerateRandomCode(int length)
		{
			var random = new Random();
			const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			string randomChars = new string(Enumerable.Repeat(chars , length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
			//return "WMK-" + randomChars;
			return randomChars;
		}

	}
}
