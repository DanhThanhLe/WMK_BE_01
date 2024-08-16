using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.UserModel;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/user")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly ISendMailService _sendMailService;

		public UserController(IUserService userService, ISendMailService sendMailService)
		{
			_userService = userService;
			_sendMailService = sendMailService;
		}
		#region Get

		[Authorize(Roles = "Admin,Manager,Staff")]
		[HttpGet("get-all")]
		public async Task<IActionResult> GetAll([FromQuery]GetAllUsersRequest? model)
		{
			if ( Request.Headers.TryGetValue("Authorization" , out var tokenHeader) )
			{
				var token = tokenHeader.ToString().Replace("Bearer " , "");

				if ( !string.IsNullOrEmpty(token) )
				{
					var result = await _userService.GetAllUsers(token, model);
					return StatusCode(result.StatusCode , result);
				}
			}

			return Unauthorized("Authorization header missing or invalid token");
		}

		//[Authorize]
		[HttpGet("get-user-token/{token}")]
		public async Task<IActionResult> GetUserByToken(string token)
		{
			var result = await _userService.GetUserByTokenAsync(token);
			return StatusCode(result.StatusCode , result);
		}

		#endregion
		//
		[Authorize(Roles = "Admin")]
		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] CreateUserRequest model)
		{
			var result = await _userService.CreateUserAsync(model);
			if ( result.Data != null )
			{
				//gửi mail cho user đó về tình hình
				_sendMailService.SendMail(result.Data.Email , "Create Account on WeMealKit" , "Your account on WemealKit with role ("
										+ result.Data.Role + ") has been created! Please contact the administrator for assistance.");
			}
			return StatusCode(result.StatusCode , result);
		}
		//
		[Authorize]
		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(Guid id , [FromBody] UpdateUserRequest model)
		{
			var result = await _userService.UpdateUserAsync(id , model);
			return StatusCode(result.StatusCode , result);
		}
		//
		[Authorize(Roles = "Admin")]
		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _userService.DeleteUserAsync(id);
			if ( result.Data != null )
			{
				//gửi mail cho user đó về tình hình
				_sendMailService.SendMail(result.Data.Email , "Remove Account" , "Your account on WemealKit with role ("
										+ result.Data.Role + ") has been removed! Please contact the administrator for assistance.");
			}
			return StatusCode(result.StatusCode , result);
		}

		//
		[Authorize(Roles = "Admin")]
		[HttpPut("change-role/{id}")]
		public async Task<IActionResult> ChangeRole(Guid id , [FromBody] ChangeRoleUserRequest model)
		{
			var result = await _userService.ChangeRoleAsync(id , model);
			if ( result.Data != null )
			{
				//gửi mail cho user đó về tình hình
				_sendMailService.SendMail(result.Data.Email , "Change role" , "Your account on WemealKit with role ("
										+ result.Data.Role + ") has been changed! Please contact the administrator for assistance.");
			}
			return StatusCode(result.StatusCode , result);
		}

		//
		[Authorize(Roles = "Admin")]
		[HttpPut("change-status/{id}")]
		public async Task<IActionResult> ChangeStatus(Guid id)
		{
			var result = await _userService.ChangeStatusAsync(id);
			if(result.Data != null)
			{
				//gửi mail cho user đó về tình hình
				_sendMailService.SendMail(result.Data.Email , "Lock Account" , "Your account on WemealKit with role (" 
										+ result.Data.Role +") has been changed to ("
										+ result.Data.Status + ")! Please contact the administrator for assistance.");
			}
			return StatusCode(result.StatusCode , result);
		}

		//
		[Authorize(Roles = "Admin")]
		[HttpPut("change-emailconfirm/{id}")]
		public async Task<IActionResult> ChangeEmailConfirm(Guid id)
		{
			var result = await _userService.ChangeEmailConfirmAsync(id);
			return StatusCode(result.StatusCode , result);
		}
	}
}
