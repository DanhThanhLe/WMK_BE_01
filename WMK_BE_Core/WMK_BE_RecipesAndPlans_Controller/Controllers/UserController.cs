using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.UserModel;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/user")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		public UserController(IUserService userService)
		{
			_userService = userService;
		}
		#region Get

		[Authorize(Roles = "Admin,Manager,Staff")]
		[HttpGet("get-all")]
		public async Task<IActionResult> GetAll()
		{
			if ( Request.Headers.TryGetValue("Authorization" , out var tokenHeader) )
			{
				var token = tokenHeader.ToString().Replace("Bearer " , "");

				if ( !string.IsNullOrEmpty(token) )
				{
					var result = await _userService.GetAllUsers(token);
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
			return StatusCode(result.StatusCode , result);
		}

		//
		[Authorize(Roles = "Admin")]
		[HttpPut("change-role/{id}")]
		public async Task<IActionResult> ChangeRole(Guid id , [FromBody] ChangeRoleUserRequest model)
		{
			var result = await _userService.ChangeRoleAsync(id , model);
			return StatusCode(result.StatusCode , result);
		}

		//
		[Authorize(Roles = "Admin")]
		[HttpPut("change-status/{id}")]
		public async Task<IActionResult> ChangeStatus(Guid id)
		{
			var result = await _userService.ChangeStatusAsync(id);
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
