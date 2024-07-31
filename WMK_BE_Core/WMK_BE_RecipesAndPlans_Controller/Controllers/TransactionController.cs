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

		[HttpPost("create_momo")]
		public async Task<IActionResult> CreateMomoPayment(OrderInfoRequest model)
		{
			var result = await _transactionService.CreatePaymentAsync(model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpGet("get-all")]
		public async Task<IActionResult> GetAllTransaction()
		{
			var result = await _transactionService.GetAllAsync();
			return StatusCode(result.StatusCode , result);	
		}


    }
}
