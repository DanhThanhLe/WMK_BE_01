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

		//[Authorize(Roles = "Admin,Manager,Staff")]
		[HttpGet("get-all")]
		public async Task<IActionResult> GetAll()
		{
			if ( Request.Headers.TryGetValue("Authorization" , out var tokenHeader) )
			{
				var token = tokenHeader.ToString().Replace("Bearer " , "");

				if ( !string.IsNullOrEmpty(token) )
				{
					var result = await _userService.GetAllUsers(token);
					return Ok(result);
				}
			}

			return Unauthorized("Authorization header missing or invalid token");
		}


		[HttpGet("get-all-staff")]
		public async Task<IActionResult> GetAllStaff()
		{
			var result = await _userService.GetAllStaffs();
			return Ok(result);
		}

		[HttpGet("get-all-shipper")]
		public async Task<IActionResult> GetAllShipper()
		{
			var result = await _userService.GetAllShippers();
			return Ok(result);
		}

		//[Authorize]
		[HttpGet("get-id/{id}")]
		public async Task<IActionResult> GetId(string id)
		{
			Guid userId;
			if ( Guid.TryParse(id , out userId) )
			{
				var result = await _userService.GetUserByIdAsync(userId);
				return Ok(result);
			}
			else
			{
				return BadRequest(new
				{
					StatusCode = 400 ,
					Message = "Invalid GUID format! Please provide a valid GUID!"
				});
			}
		}


		[Authorize]
		[HttpGet("get-user/{emailOrUsername}")]
		public async Task<IActionResult> GetUser(string emailOrUsername)
		{
			var result = await _userService.GetUserAsync(emailOrUsername);
			return Ok(result);
		}

		//[Authorize]
		[HttpGet("get-user-token/{token}")]
		public async Task<IActionResult> GetUserByToken(string token)
		{
			var result = await _userService.GetUserByTokenAsync(token);
			return Ok(result);
		}


		[Authorize(Roles = "Admin")]
		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] CreateUserRequest model)
		{
			var result = await _userService.CreateUserAsync(model);
			return Ok(result);
		}

		[Authorize]
		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(Guid id , [FromBody] UpdateUserRequest model)
		{
			var result = await _userService.UpdateUserAsync(id , model);
			return Ok(result);
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _userService.DeleteUserAsync(id);
			return Ok(result);
		}


		[Authorize(Roles = "Admin")]
		[HttpPut("change-role/{id}")]
		public async Task<IActionResult> ChangeRole(Guid id , [FromBody] ChangeRoleUserRequest model)
		{
			var result = await _userService.ChangeRoleAsync(id , model);
			return Ok(result);
		}


		[Authorize(Roles = "Admin")]
		[HttpPut("change-status/{id}")]
		public async Task<IActionResult> ChangeStatus(Guid id)
		{
			var result = await _userService.ChangeStatusAsync(id);
			return Ok(result);
		}


		[Authorize(Roles = "Admin")]
		[HttpPut("change-emailconfirm/{id}")]
		public async Task<IActionResult> ChangeEmailConfirm(Guid id)
		{
			var result = await _userService.ChangeEmailConfirmAsync(id);
			return Ok(result);
		}
	}
}
