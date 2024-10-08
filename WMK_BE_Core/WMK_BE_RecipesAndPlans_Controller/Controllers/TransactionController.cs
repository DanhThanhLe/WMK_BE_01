﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/transaction")]
	[ApiController]
	public class TransactionController : ControllerBase
	{
		private readonly ITransactionService _transactionService;
		private readonly ISendMailService _sendMailService;

		public TransactionController(ITransactionService transactionService , ISendMailService sendMailService)
		{
			_transactionService = transactionService;
			_sendMailService = sendMailService;
		}

		[HttpGet("get-all")]
		[Authorize]
		public async Task<IActionResult> GetAllTransaction()
		{
			var result = await _transactionService.GetAllAsync();
			return StatusCode(result.StatusCode , result);
		}

		[HttpPost("create-new")]
		[Authorize]
		public async Task<IActionResult> UpdateTransactionListForOrder([FromBody] CreatePaymentRequest request)
		{
			var result = await _transactionService.CreateNewPaymentAsync(request);
			return StatusCode(result.StatusCode , result);
		}

		//refund
		[HttpPost("refund-zalopay")]
		[Authorize]
		public async Task<IActionResult> RefundZaloPay([FromBody] RefundZaloPayRequest request)
		{
			var result = await _transactionService.RefundTransactionAsync(request);
			//thông báo trạng thái cho customer
			if ( result.Data != null && !result.Data.EmailCustomer.IsNullOrEmpty() && !result.Data.OrderCode.IsNullOrEmpty() )
			{
				if ( result.StatusCode == 200 )
				{
					_sendMailService.SendMail(result.Data.EmailCustomer , "Refund on WeMealKit" , "Your order "
										+ result.Data.OrderCode + " has been refund successfull.");
				}
			}
			return StatusCode(result.StatusCode , result);
		}

	}
}
