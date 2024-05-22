using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public async Task<ResponseObject<List<UsersResponse>>> GetAllUsers()
		{
			var result = new ResponseObject<List<UsersResponse>>();
			var users = await _unitOfWork.UserRepository.GetAllAsync();
			if ( users != null && users.Count() > 0 )
			{
				var usersModel = _mapper.Map<List<UsersResponse>>(users);
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
		//By Admin
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
				result.Message = "Email have exist!";
				return result;
			}
			//check role 
			if ( !Enum.IsDefined(typeof(Role) , model.Role) )
			{
				result.StatusCode = 404;
				result.Message = "Role doesn't exist!";
				return result;
			}
			//check gender 
			if ( !Enum.IsDefined(typeof(Gender) , model.Gender) )
			{
				result.StatusCode = 404;
				result.Message = "Gender doesn't exist!";
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
				result.Message = "Created user (" + newUser.Email + ") successfully with password (" + defaultPassword + ").";
				//result.Data = new CreateUserModelResponse { Id = newUser.Id, UserName = newUser.UserName };
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Failed to create new User!";
				return result;
			}
		}
		public async Task<ResponseObject<BaseUserResponse>> UpdateUserAsync(UpdateUserRequest model)
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
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(model.Id);
			if ( userExist == null )
			{
				result.StatusCode = 404;
				result.Message = "User not found!";
				return result;
			}
			if ( !string.IsNullOrEmpty(model.Email) )
			{
				//check email exist
				var emailExist = await _unitOfWork.UserRepository.GetByEmailOrUserNameAsync(model.Email);
				if ( emailExist != null )
				{
					result.StatusCode = 402;
					result.Message = "Email have exist!";
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
					result.Message = "Username have exist!";
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
			if ( model.DateOfBirth != null )
				userExist.DateOfBirth = model.DateOfBirth;
			if ( !string.IsNullOrEmpty(model.Address) )
				userExist.Address = model.Address;
			if ( model.Gender != null )
			{
				if ( !Enum.IsDefined(typeof(Gender) , model.Gender) )
				{
					result.StatusCode = 404;
					result.Message = "Gender doesn't exist!";
					return result;
				}
				userExist.Gender = (Gender)model.Gender;
			}
			var updateResult = await _unitOfWork.UserRepository.UpdateAsync(userExist);
			if ( updateResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Updated user (" + userExist.Email + ") successfully.";
				result.Data = new BaseUserResponse { Id = userExist.Id , Email = userExist.Email };
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Failed to update User!";
				return result;
			}
		}
		public async Task<ResponseObject<BaseUserResponse>> DeleteUserAsync(IdUserRequest model)
		{
			var result = new ResponseObject<BaseUserResponse>();
			var validationResult = _idUserValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			//check user exists
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(model.Id);
			if ( userExist == null )
			{
				result.StatusCode = 404;
				result.Message = "User not found!";
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
					result.Message = "User (" + userExist.Email + ")have order trust change status (UnActive) successfully.";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Failed to Change User status to UnActive!";
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
					result.Message = "User (" + userExist.Email + ") deleted successfully.";
					// result.Data = new DeleteUserModelResponse { Id = userExist.Id, UserName = userExist.UserName };
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Failed to delete User!";
					return result;
				}
			}
		}
		public async Task<ResponseObject<BaseUserResponse>> ChangeRoleAsync(ChangeRoleUserRequest model)
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
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(model.Id);
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
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "User not Exist!";
				return result;
			}
		}
		public async Task<ResponseObject<BaseUserResponse>> ChangeStatusAsync(IdUserRequest model)
		{
			var result = new ResponseObject<BaseUserResponse>();
			//check validate
			var validationResult = _idUserValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			//check User exist
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(model.Id);
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
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Changed User (" + userExist.UserName + ") with status (" + userExist.Status + ") successfully.";
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
		public async Task<ResponseObject<BaseUserResponse>> ChangeEmailConfirmAsync(IdUserRequest model)
		{
			var result = new ResponseObject<BaseUserResponse>();
			//check validate
			var validationResult = _idUserValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			//check User exist
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(model.Id.ToString());
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
	}
}
