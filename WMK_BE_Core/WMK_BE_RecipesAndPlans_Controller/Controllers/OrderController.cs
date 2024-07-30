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
        public async Task<IActionResult> GetAll()
        {
            var result = await _orderService.GetAllOrders();
            return Ok(result);  
        }

        [HttpGet("get-by-userid/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            Guid idConvert;
            if (Guid.TryParse(userId, out idConvert))
            {
                var result = await _orderService.GetOrdersByUserId(idConvert);
                return Ok(result);
            }
            else
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid GUID format! Please provide a valid GUID!"
                });
            }
        }

		[HttpGet("get-order/{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _orderService.GetOrderByIdAsync(id);
			return Ok(result);
		}

		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] CreateOrderRequest model)
		{
			var result = await _orderService.CreateOrderAsync(model);
			return Ok(result);
		}

		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(string id,[FromBody] UpdateOrderRequest model)
		{
			var result = await _orderService.UpdateOrderAsync(id,model);
			return Ok(result);
		}

		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _orderService.DeleteOrderAsync(id);
			return Ok(result);
		}

		[HttpPut("change-status/{id}")]
		public async Task<IActionResult> ChangeStatus(Guid id,[FromQuery] ChangeStatusOrderRequest model)
		{
			var result = await _orderService.ChangeStatusOrderAsync(id,model);
			return Ok(result);
		}
	}
}
