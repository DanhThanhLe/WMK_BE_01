using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel;
using WMK_BE_BusinessLogic.Service.Implement;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/order")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;
		public OrderController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		[HttpGet("get-all")]
		//[Authorize(Roles = "Admin,Manager,Staff")]
		public async Task<IActionResult> GetAll()
		{
			var result = await _orderService.GetAllOrders();
			return StatusCode(result.StatusCode , result);
		}

		[HttpGet("get-by-userid/{userId}")]
		[Authorize]
		public async Task<IActionResult> GetByUserId(string userId)
		{
			Guid idConvert;
			if ( Guid.TryParse(userId , out idConvert) )
			{
				var result = await _orderService.GetOrdersByUserId(idConvert);
				return StatusCode(result.StatusCode , result);
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

		[HttpGet("get-order/{id}")]
		//[Authorize]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _orderService.GetOrderByIdAsync(id);
			return StatusCode(result.StatusCode , result);
		}

		[HttpPost("create")]
		//[Authorize]
		public async Task<IActionResult> Create([FromBody] CreateOrderRequest model)
		{
			var result = await _orderService.CreateOrderAsync(model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpPut("update/{id}")]
		//[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> Update(string id , [FromBody] UpdateOrderRequest model)
		{
			var result = await _orderService.UpdateOrderAsync(id , model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpPut("change-status/{id}")]
		//[Authorize]
		public async Task<IActionResult> ChangeStatus(Guid id , [FromQuery] ChangeStatusOrderRequest model)
		{
			var result = await _orderService.ChangeStatusOrderAsync(id , model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpPut("change-ordergroup/{idOrder}")]
		//[Authorize]
		public async Task<IActionResult> ChangeOrdergroup(Guid idOrder , [FromBody] Guid idOrderGroup)
		{
			var result = await _orderService.ChangeOrderGroupAsync(idOrder , idOrderGroup);
			return StatusCode(result.StatusCode , result);
		}

		[HttpDelete("delete/{id}")]
		[Authorize]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _orderService.DeleteOrderAsync(id);
			return StatusCode(result.StatusCode , result);
		}
	}
}
