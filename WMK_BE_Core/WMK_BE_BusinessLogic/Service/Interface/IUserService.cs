using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.UserModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.UserModel;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface IUserService
	{
		Task<ResponseObject<List<UsersResponse>>> GetAllUsers();
		Task<ResponseObject<UserResponse?>> GetUserByIdAsync(Guid id);
		Task<ResponseObject<UserResponse?>> GetUserAsync(string email);
		Task<ResponseObject<BaseUserResponse>> CreateUserAsync(CreateUserRequest model);
		Task<ResponseObject<BaseUserResponse>> UpdateUserAsync(UpdateUserRequest model);
		Task<ResponseObject<BaseUserResponse>> DeleteUserAsync(IdUserRequest model);
		Task<ResponseObject<BaseUserResponse>> ChangeRoleAsync(ChangeRoleUserRequest model);
		Task<ResponseObject<BaseUserResponse>> ChangeStatusAsync(ChangeStatusUserRequest model);
		Task<ResponseObject<BaseUserResponse>> ChangeEmailConfirmAsync(IdUserRequest model);

	}
}
