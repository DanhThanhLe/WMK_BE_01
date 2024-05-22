using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.AuthModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.UserModel;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface IAuthService
	{
		Task<ResponseObject<UserResponse>> LoginAsync(LoginModel model);
		Task<ResponseObject<UserResponse>> RegisterEmailAsync(RegisterModel model);
		Task<ResponseObject<BaseUserResponse>> CheckEmailConfirmAsync(CheckEmailConfirmRequest model);
		Task<ResponseObject<BaseUserResponse>> ResetPasswordAsync(ResetPasswordRequest model);
		Task<ResponseObject<BaseUserResponse>> ResetPasswordEmailAsync(ResetPasswordByEmailRequest model);
		Task<ResponseObject<BaseUserResponse>> SendCodeResetEmailAsync(SendCodeByEmailRequest model);

	}
}
