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

		[HttpGet("get-all")]
		public async Task<IActionResult> Get()
		{
			var result = await _userService.GetAllUsers();
			return Ok(result);
		}

		[HttpGet("get-id")]
		public async Task<IActionResult> GetId([FromQuery] Guid id)
		{
			var result = await _userService.GetUserByIdAsync(id);
			return Ok(result);
		}

		[HttpGet("get-user")]
		public async Task<IActionResult> GetUser([FromQuery] string id)
		{
			var result = await _userService.GetUserAsync(id);
			return Ok(result);
		}

		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] CreateUserRequest model)
		{
			var result = await _userService.CreateUserAsync(model);
			return Ok(result);
		}

		[HttpPut("update")]
		public async Task<IActionResult> Update([FromBody] UpdateUserRequest model)
		{
			var result = await _userService.UpdateUserAsync(model);
			return Ok(result);
		}

		[HttpDelete("delete")]
		public async Task<IActionResult> Delete([FromQuery] IdUserRequest model)
		{
			var result = await _userService.DeleteUserAsync(model);
			return Ok(result);
		}
		[HttpPut("change-role")]
		public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleUserRequest model)
		{
			var result = await _userService.ChangeRoleAsync(model);
			return Ok(result);
		}
		[HttpPut("change-status")]
		public async Task<IActionResult> ChangeStatus([FromBody] ChangeStatusUserRequest model)
		{
			var result = await _userService.ChangeStatusAsync(model);
			return Ok(result);
		}
		[HttpPut("change-emailconfirm")]
		public async Task<IActionResult> ChangeEmailConfirm([FromQuery] IdUserRequest model)
		{
			var result = await _userService.ChangeEmailConfirmAsync(model);
			return Ok(result);
		}
	}
}
