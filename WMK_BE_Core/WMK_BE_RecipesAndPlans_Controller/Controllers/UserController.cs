using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
			var result = await _userService.GetAllUsers();
			return Ok(result);
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
		[HttpGet("get-id")]
		public async Task<IActionResult> GetId([FromQuery] string id)
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
		[HttpGet("get-user")]
		public async Task<IActionResult> GetUser([FromQuery] string emailOrUsername)
		{
			var result = await _userService.GetUserAsync(emailOrUsername);
			return Ok(result);
		}
		
		//[Authorize]
		[HttpGet("get-user-token")]
		public async Task<IActionResult> GetUserByToken([FromQuery] string token)
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
		[HttpPut("update")]
		public async Task<IActionResult> Update([FromBody] UpdateUserRequest model)
		{
			var result = await _userService.UpdateUserAsync(model);
			return Ok(result);
		}
		
		[Authorize(Roles = "Admin")]
		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete( Guid id)
		{
			var model = new IdUserRequest();
			model.Id = id.ToString();
			var result = await _userService.DeleteUserAsync(model);
			return Ok(result);
		}

		
		[Authorize(Roles = "Admin")]
		[HttpPut("change-role")]
		public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleUserRequest model)
		{
			var result = await _userService.ChangeRoleAsync(model);
			return Ok(result);
		}

		
		[Authorize(Roles = "Admin")]
		[HttpPut("change-status")]
		public async Task<IActionResult> ChangeStatus([FromBody] IdUserRequest model)
		{
			var result = await _userService.ChangeStatusAsync(model);
			return Ok(result);
		}

		
		[Authorize(Roles = "Admin")]
		[HttpPut("change-emailconfirm")]
		public async Task<IActionResult> ChangeEmailConfirm([FromQuery] IdUserRequest model)
		{
			var result = await _userService.ChangeEmailConfirmAsync(model);
			return Ok(result);
		}
	}
}
