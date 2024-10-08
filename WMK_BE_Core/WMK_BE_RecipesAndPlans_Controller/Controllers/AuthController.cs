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
		private readonly ISendMailService _sendMailService;

		public AuthController(IAuthService authService , ISendMailService sendMailService)
		{
			_authService = authService;
			_sendMailService = sendMailService;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginModel model)
		{
			var result = await _authService.LoginAsync(model);
			if ( result.StatusCode == 405 )
			{
				if ( result.Data != null )
				{
					_sendMailService.SendMail(result.Data.Email , "Confirm Mail", "Đoạn code này dùng để xác minh tài khoản trong hệ thống WEMEALKIT: " + result.Message);
				}
				return StatusCode(405 , new { statusCode = 405 , message = "Hãy kiểm tra email đăng kí để lấy mã xác nhận tài khoản!" });

			}
			return StatusCode(result.StatusCode , result);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterModel model)
		{
			var result = await _authService.RegisterEmailAsync(model);
			if(result.StatusCode == 200 )
			{
				_sendMailService.SendMail(model.Email , "Confirm Mail" , "Đoạn code này dùng để xác minh tài khoản trong hệ thống WEMEALKIT: " + result.Message);
				return StatusCode(200 , new { statusCode = 200 , message = "Đăng kí thành công. Bạn hãy kiểm tra email đã đăng ký để lấy mã xác nhận tài khoản!" });//Register successfully. Please check mail
                /*
				 _sendMailService.SendMail(model.Email , "Confirm Mail" , "This code is for authentication in WEMEALKIT: " + result.Message);
				return StatusCode(200 , new { statusCode = 200 , message = "Register successfully. Please check mail!" });
				 */

            }
            return StatusCode(result.StatusCode , result);
		}

		[HttpGet("confirm-mail")]
		public async Task<IActionResult> ComfirmMail([FromQuery] CheckEmailConfirmRequest model)
		{
			var result = await _authService.CheckEmailConfirmAsync(model);
			return StatusCode(result.StatusCode , result);
		}
		
		[HttpPut("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
		{
			var result = await _authService.ResetPasswordAsync(model);
			return StatusCode(result.StatusCode , result);
		}
		
		[HttpPut("reset-password-email")]
		public async Task<IActionResult> ResetPasswordByEmail([FromBody] ResetPasswordByEmailRequest model)
		{
			var result = await _authService.ResetPasswordEmailAsync(model);
			return StatusCode(result.StatusCode , result);
		}
		
		[HttpPost("send-code")]
		public async Task<IActionResult> SendCode([FromBody] SendCodeByEmailRequest model)
		{
			var result = await _authService.SendCodeResetEmailAsync(model);
			return StatusCode(result.StatusCode , result);
		}
	}
}
