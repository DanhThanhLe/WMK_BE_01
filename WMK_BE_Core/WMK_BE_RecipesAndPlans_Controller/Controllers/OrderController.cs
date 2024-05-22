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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest model)
        {
            var result = await _orderService.CreateOrderAsync(model);
            return Ok(result);
        }
    }
}
