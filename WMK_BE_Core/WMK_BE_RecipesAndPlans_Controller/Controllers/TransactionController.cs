using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/transaction")]
	[ApiController]
	public class TransactionController : ControllerBase
	{
		private readonly ITransactionService _transactionService;

		public TransactionController(ITransactionService transactionService)
		{
			_transactionService = transactionService;
		}

		//[HttpPost("create_momo")]
		//[Authorize]
		//public async Task<IActionResult> CreateMomoPayment(OrderInfoRequest model)
		//{
		//	var result = await _transactionService.CreatePaymentAsync(model);
		//	return StatusCode(result.StatusCode , result);
		//}

		[HttpGet("get-all")]
		[Authorize]
		public async Task<IActionResult> GetAllTransaction()
		{
			var result = await _transactionService.GetAllAsync();
			return StatusCode(result.StatusCode , result);
		}

		//
		[HttpPost("create-new")]
		//[Authorize]
		public async Task<IActionResult> UpdateTransactionListForOrder([FromBody] CreatePaymentRequest request)
		{
			var result = await _transactionService.CreateNewPaymentAsync(request);
			return StatusCode(result.StatusCode , result);
		}


		//refund
		[HttpPost("refund-zalopay")]
		//[Authorize]
		public async Task<IActionResult> RefundZaloPay([FromBody] RefundZaloPayRequest request)
		{
			var result = await _transactionService.RefundTransactionAsync(request);
			return StatusCode(result.StatusCode , result);
		}

	}
}
