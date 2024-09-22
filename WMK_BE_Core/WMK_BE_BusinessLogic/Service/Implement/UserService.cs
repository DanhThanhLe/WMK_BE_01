using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.UserModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.UserModel;
using WMK_BE_BusinessLogic.Helpers;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class UserService : IUserService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		#region Validator
		private readonly CreateUserModelValidator _createValidator;
		private readonly UpdateModelValidator _updateValidator;
		private readonly IdUserModelValidator _idUserValidator;
		private readonly ChangeRoleUserModelValidator _changeRoleValidator;

		#endregion
		public UserService(IUnitOfWork unitOfWork , IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;

			_createValidator = new CreateUserModelValidator();
			_updateValidator = new UpdateModelValidator();
			_idUserValidator = new IdUserModelValidator();
			_changeRoleValidator = new ChangeRoleUserModelValidator();
		}
		#region Get
		public async Task<ResponseObject<List<UsersResponse>>> GetAllUsers(string tokenHeader , GetAllUsersRequest? model)
		{
			var result = new ResponseObject<List<UsersResponse>>();
			try
			{
				// Read token
				var handler = new JwtSecurityTokenHandler();
				var tokenString = handler.ReadToken(tokenHeader) as JwtSecurityToken;
				if ( tokenString != null )
				{
					// Get user id from token
					var userIdClaim = tokenString.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
					if ( userIdClaim != null )
					{
						var userId = userIdClaim.Value;
						var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
						if ( user != null )
						{
							List<User> usersList;

							if ( user.Role == Role.Manager )
							{
								// For Manager and Staff roles, fetch only Staff and Shipper
								usersList = await _unitOfWork.UserRepository.GetAllAsync();
								usersList = usersList.Where(u => u.Role == Role.Staff || u.Role == Role.Shipper || u.Role == Role.Customer).ToList();
							}
							else if ( user.Role == Role.Staff )
							{
								// For Manager and Staff roles, fetch only Staff and Shipper
								usersList = await _unitOfWork.UserRepository.GetAllAsync();
								usersList = usersList.Where(u => u.Role == Role.Customer || u.Role == Role.Shipper).ToList();
							}
							else if ( user.Role == Role.Admin )
							{
								// For Admin role, fetch all users
								usersList = await _unitOfWork.UserRepository.GetAllAsync();
							}
							else
							{
								result.StatusCode = 403;
								result.Message = "Don't have permission!";
								return result;
							}

							var usersModel = _mapper.Map<List<UsersResponse>>(usersList);

							if ( model != null && !string.IsNullOrEmpty(model.Sreach) )
							{
								// Apply search filter
								usersModel = usersModel.Where(u =>
									u.UserName.ToLower().RemoveDiacritics().Contains(model.Sreach.ToLower().RemoveDiacritics()) ||
									u.Email.ToLower().RemoveDiacritics().Contains(model.Sreach.ToLower().RemoveDiacritics()) ||
									u.FirstName.ToLower().RemoveDiacritics().Contains(model.Sreach.ToLower().RemoveDiacritics()) ||
									u.LastName.ToLower().RemoveDiacritics().Contains(model.Sreach.ToLower().RemoveDiacritics())
								).ToList();
							}

							result.StatusCode = 200;
							result.Message = "Get users success (" + usersModel.Count() + ")";
							result.Data = usersModel.OrderBy(u => u.Email).ToList();
						}
						else
						{
							result.StatusCode = 404;
							result.Message = "User not found!";
							result.Data = [];
						}
					}
					else
					{
						result.StatusCode = 401;
						result.Message = "Token does not have userId!";
					}
				}
				else
				{
					result.StatusCode = 402;
					result.Message = "Token is null!";
				}
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = "Error when processing: " + ex.Message;
			}
			return result;
		}

		public async Task<ResponseObject<UserResponse?>> GetUserAsync(string emailOrUsername)
		{
			var result = new ResponseObject<UserResponse?>();
			var userExist = await _unitOfWork.UserRepository.GetByEmailOrUserNameAsync(emailOrUsername);
			if ( userExist != null )
			{
				if ( userExist.Status == WMK_BE_RecipesAndPlans_DataAccess.Enums.BaseStatus.UnAvailable )
				{
					result.StatusCode = 300;
					result.Message = "User UnActive!";
					return result;
				}
				var userModel = _mapper.Map<UserResponse>(userExist);
				result.StatusCode = 200;
				result.Message = "Success";
				result.Data = userModel;
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "User not found!";
				return result;
			}
		}
		public async Task<ResponseObject<UserResponse?>> GetUserByIdAsync(Guid id)
		{
			var result = new ResponseObject<UserResponse?>();
			var user = await _unitOfWork.UserRepository.GetByIdAsync(id.ToString());
			if ( user != null )
			{
				if ( user.Status == WMK_BE_RecipesAndPlans_DataAccess.Enums.BaseStatus.UnAvailable )
				{
					result.StatusCode = 300;
					result.Message = "User UnActive!";
					return result;
				}
				var userModel = _mapper.Map<UserResponse>(user);
				result.StatusCode = 200;
				result.Message = "User";
				result.Data = userModel;
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "User not exist!";
				return result;
			}
		}
		//get user by role (staff, shipper)
		public async Task<ResponseObject<List<UsersResponse>>> GetAllStaffs()
		{
			var result = new ResponseObject<List<UsersResponse>>();
			var users = await _unitOfWork.UserRepository.GetAllAsync();
			if ( users != null && users.Count() > 0 )
			{
				var staffList = users.Where(u => u.Role == Role.Staff).ToList();
				var usersModel = _mapper.Map<List<UsersResponse>>(staffList);
				result.StatusCode = 200;
				result.Message = "Success";
				result.Data = usersModel;
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Don't have user!";
				return result;
			}
		}

		public async Task<ResponseObject<List<UsersResponse>>> GetAllShippers()
		{
			var result = new ResponseObject<List<UsersResponse>>();
			var users = await _unitOfWork.UserRepository.GetAllAsync();
			if ( users != null && users.Count() > 0 )
			{
				var shipperList = users.Where(u => u.Role == Role.Shipper).ToList();
				var usersModel = _mapper.Map<List<UsersResponse>>(shipperList);
				result.StatusCode = 200;
				result.Message = "Success";
				result.Data = usersModel;
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Don't have user!";
				return result;
			}
		}
		//get user by token
		public async Task<ResponseObject<UserResponse>> GetUserByTokenAsync(string token)
		{
			var result = new ResponseObject<UserResponse>();
			try
			{
				//read token
				var handler = new JwtSecurityTokenHandler();
				var tokenString = handler.ReadToken(token) as JwtSecurityToken;
				if ( tokenString != null )
				{
					//get user id from token
					var userIdClaim = tokenString.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
					if ( userIdClaim != null )
					{
						var userId = userIdClaim.Value;
						var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
						if ( user == null )
						{
							result.StatusCode = 404;
							result.Message = "User not found!";
							return result;
						}
						else
						{
							var rolesClaim = tokenString.Claims.Where(claim => claim.Type == ClaimTypes.Role).Select(claim => claim.Value);
							var userModel = _mapper.Map<UserResponse>(user);
							result.StatusCode = 200;
							result.Message = "User Profile";
							result.Data = userModel;
							return result;
						}
					}
					else
					{
						result.StatusCode = 401;
						result.Message = "Token not have userId!";
						return result;
					}
				}
				else
				{
					result.StatusCode = 402;
					result.Message = "Token invalid!";
					return result;
				}
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = "Error when processing: " + ex.Message;
				return result;
			}
		}
		#endregion

		public async Task<ResponseObject<BaseUserResponse>> CreateUserAsync(CreateUserRequest model)
		{
			var result = new ResponseObject<BaseUserResponse>();
			//check validate
			var validationResult = _createValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			//check email exist
			var emailExist = await _unitOfWork.UserRepository.GetByEmailOrUserNameAsync(model.Email);
			if ( emailExist != null )
			{
				result.StatusCode = 402;
				result.Message = "Email không tồn tại";
				return result;
			}
			//check role 
			if ( !Enum.IsDefined(typeof(Role) , model.Role) )
			{
				result.StatusCode = 404;
				result.Message = "Role không tồn tại";
				return result;
			}
			//check gender 
			if ( !Enum.IsDefined(typeof(Gender) , model.Gender) )
			{
				result.StatusCode = 404;
				result.Message = "Giới tính không tồn tại!";
				return result;
			}

			var newUser = _mapper.Map<User>(model);
			newUser.EmailConfirm = EmailConfirm.NotConfirm;//set not confirmEmail
			newUser.UserName = model.Email;
			//setup password
			var defaultPassword = "User123@";
			newUser.PasswordHash = HashHelper.GetSignature256(defaultPassword);
			var createResult = await _unitOfWork.UserRepository.CreateAsync(newUser);
			if ( createResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Tạo user (" + newUser.Email + ") thành công với mật khẩu mặc định (" + defaultPassword + ").";
				result.Data = _mapper.Map<BaseUserResponse>(newUser);
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Tạo người dùng thất bại";
				return result;
			}
		}

		public async Task<ResponseObject<BaseUserResponse>> UpdateUserAsync(Guid idUser , UpdateUserRequest model)
		{
			var result = new ResponseObject<BaseUserResponse>();
			var validationResult = _updateValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			//check user exists
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(idUser.ToString());
			if ( userExist == null )
			{
				result.StatusCode = 404;
				result.Message = "không tìm thấy người dùng";
				return result;
			}
			if ( !string.IsNullOrEmpty(model.Email) )
			{
				//check email exist
				var emailExist = await _unitOfWork.UserRepository.GetByEmailOrUserNameAsync(model.Email);
				if ( emailExist != null )
				{
					result.StatusCode = 402;
					result.Message = "Email này đã được đăng kí";
					return result;
				}
				userExist.Email = model.Email;
				userExist.EmailConfirm = EmailConfirm.NotConfirm;
			}
			if ( !string.IsNullOrEmpty(model.UserName) )
			{
				//check username exist
				var usernameExist = await _unitOfWork.UserRepository.GetByEmailOrUserNameAsync(model.UserName);
				if ( usernameExist != null )
				{
					result.StatusCode = 402;
					result.Message = "Tên người dùng đã tồn tại";
					return result;
				}
				userExist.UserName = model.UserName;
			}
			if ( !string.IsNullOrEmpty(model.Phone) )
			{
				userExist.Phone = model.Phone;
			}
			if ( !string.IsNullOrEmpty(model.FirstName) )
				userExist.FirstName = model.FirstName;
			if ( !string.IsNullOrEmpty(model.LastName) )
				userExist.LastName = model.LastName;
			if ( !string.IsNullOrEmpty(model.Address) )
				userExist.Address = model.Address;
			if ( model.Gender != null )
			{
				if ( !Enum.IsDefined(typeof(Gender) , model.Gender) )
				{
					result.StatusCode = 404;
					result.Message = "Giới tính không tồn tại";
					return result;
				}
				userExist.Gender = (Gender)model.Gender;
			}
			var updateResult = await _unitOfWork.UserRepository.UpdateAsync(userExist);
			if ( updateResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Cập nhật người dùng thành công";
				result.Data = _mapper.Map<BaseUserResponse>(userExist);
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Cập nhật người dùng không thành công";
				return result;
			}
		}

		public async Task<ResponseObject<BaseUserResponse>> DeleteUserAsync(Guid id)
		{
			var result = new ResponseObject<BaseUserResponse>();
			//check user exists
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(id.ToString());
			if ( userExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Không tìm thấy người dùng";
				return result;
			}
			//If user have order trust change status
			var orderExist = await _unitOfWork.UserRepository.GetOrderExistInUserAsync(userExist.Id);
			if ( orderExist )
			{
				userExist.Status = BaseStatus.UnAvailable;
				var updateResult = await _unitOfWork.UserRepository.UpdateAsync(userExist);
				if ( updateResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Chuyển trạng thái thành công";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Chuyển trạng thái không thành công";
					return result;
				}
			}
			else
			{
				var deleteResult = await _unitOfWork.UserRepository.DeleteAsync(userExist.Id.ToString());
				if ( deleteResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Xóa thành công nguời dùng";
					// result.Data = new DeleteUserModelResponse { Id = userExist.Id, UserName = userExist.UserName };
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Xóa người dùng không thành công";
					return result;
				}
			}
		}

		#region Change
		public async Task<ResponseObject<BaseUserResponse>> ChangeRoleAsync(Guid idUser , ChangeRoleUserRequest model)
		{
			var result = new ResponseObject<BaseUserResponse>();
			var validationResult = _changeRoleValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(idUser.ToString());
			if ( userExist != null )
			{
				//check role exist
				if ( !Enum.IsDefined(typeof(Role) , model.NewRole) )
				{
					result.StatusCode = 404;
					result.Message = "Role doesn't exist!";
					return result;
				}

				// Add new role
				userExist.Role = model.NewRole;
				var addResult = await _unitOfWork.UserRepository.UpdateAsync(userExist);
				if ( !addResult )
				{
					result.StatusCode = 500;
					result.Message = "Failed to add new role!";
					return result;
				}
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Changed role for user (" + userExist.UserName + ") to " + model.NewRole + " successfully.";
				result.Data = _mapper.Map<BaseUserResponse>(userExist);
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "User not Exist!";
				return result;
			}
		}

		public async Task<ResponseObject<BaseUserResponse>> ChangeStatusAsync(Guid id)
		{
			var result = new ResponseObject<BaseUserResponse>();
			//check User exist
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(id.ToString());
			if ( userExist == null )
			{
				result.StatusCode = 404;
				result.Message = "User not found!";
				return result;
			}

			if ( userExist.Status == BaseStatus.Available )
			{
				userExist.Status = BaseStatus.UnAvailable;
				var updateResult = await _unitOfWork.UserRepository.UpdateAsync(userExist);
				if ( updateResult )
				{
					//nếu là shipper thì thông báo cho shipper đó và shipper đó sẽ bị xóa khỏi order group
					if ( userExist.Role == Role.Shipper )
					{
						userExist.OrderGroup = null;
						var orderGroupExist = _unitOfWork.OrderGroupRepository.Get(og => og.ShipperId == userExist.Id).FirstOrDefault();
						if ( orderGroupExist != null )
						{
							//chỉ tắt orderGroup đi để có trường hợp đối chiếu lại 
							orderGroupExist.Status = BaseStatus.UnAvailable;
							orderGroupExist.ShipperId = null;
						}
					}
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Changed User (" + userExist.UserName + ") with status (" + userExist.Status + ") successfully.";
					result.Data = _mapper.Map<BaseUserResponse>(userExist);
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Update status user unsuccess!";
					return result;
				}
			}
			else
			{
				userExist.Status = BaseStatus.Available;
				var updateResult = await _unitOfWork.UserRepository.UpdateAsync(userExist);
				if ( updateResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Changed User (" + userExist.UserName + ") with status (" + userExist.Status + ") successfully.";
					result.Data = _mapper.Map<BaseUserResponse>(userExist);
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Update status user unsuccess!";
					return result;
				}
			}

		}
		//admin
		public async Task<ResponseObject<BaseUserResponse>> ChangeEmailConfirmAsync(Guid id)
		{
			var result = new ResponseObject<BaseUserResponse>();
			//check User exist
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(id.ToString());
			if ( userExist == null )
			{
				result.StatusCode = 404;
				result.Message = "User not Exist!";
				return result;
			}
			if ( userExist.EmailConfirm == EmailConfirm.NotConfirm )
			{
				userExist.EmailConfirm = EmailConfirm.Confirm;
				var updateResult = await _unitOfWork.UserRepository.UpdateAsync(userExist);
				if ( updateResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Changed User (" + userExist.UserName + ") with confirm email successfully.";
					result.Data = _mapper.Map<BaseUserResponse>(userExist);
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Update email confirm user unsuccess!";
					return result;
				}
			}
			else
			{
				userExist.EmailConfirm = EmailConfirm.Confirm;
				var updateResult = await _unitOfWork.UserRepository.UpdateAsync(userExist);
				if ( updateResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Changed User (" + userExist.UserName + ") with not confirm email successfully.";
					result.Data = _mapper.Map<BaseUserResponse>(userExist);
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Update not email confirm user unsuccess!";
					return result;
				}
			}
		}
		#endregion

	}
}
