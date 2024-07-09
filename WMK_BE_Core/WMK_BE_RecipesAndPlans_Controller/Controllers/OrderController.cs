using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel;
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

        [HttpGet("get-order")]
        public async Task<IActionResult> GetById([FromQuery] IdOrderRequest model)
        {
            var result = await _orderService.GetOrderByIdAsync(model);
            return Ok(result);  
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest model)
        {
            var result = await _orderService.CreateOrderAsync(model);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateOrderRequest model)
        {
            var result = await _orderService.UpdateOrderAsync(model);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] IdOrderRequest model)
        {
            var result = await _orderService.DeleteOrderAsync(model);
            return Ok(result);
        }

        [HttpPut("change-status")]
        public async Task<IActionResult> ChangeStatus([FromQuery] ChangeStatusOrderRequest model)
        {
            var result = await _orderService.ChangeStatusOrderAsync(model);
            return Ok(result);
        }
    }
}
