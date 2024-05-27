﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.AuthModel;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody]LoginModel model)
		{
			var result = await _authService.LoginAsync(model);
			if(result.StatusCode == 405 )
			{
				return BadRequest();
			}
			return Ok(result);
		}
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody]RegisterModel model)
		{
			var result = await _authService.RegisterEmailAsync(model);
			return Ok(result);
		}
		[HttpGet("confirm-mail")]
		public async Task<IActionResult> ComfirmMail([FromBody]CheckEmailConfirmRequest model)
		{
			var result = await _authService.CheckEmailConfirmAsync(model);
			return Ok(result);
		}
		[HttpPut("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordRequest model)
		{
			var result = await _authService.ResetPasswordAsync(model);
			return Ok(result);
		}
		[HttpPut("reset-password-email")]
		public async Task<IActionResult> ResetPasswordByEmail([FromBody]ResetPasswordByEmailRequest model)
		{
			var result = await _authService.ResetPasswordEmailAsync(model);
			return Ok(result);
		}
		[HttpPost("send-code")]
		public async Task<IActionResult> SendCode([FromBody]SendCodeByEmailRequest model)
		{
			var result = await _authService.SendCodeResetEmailAsync(model);
			return Ok(result);
		}


    }
}