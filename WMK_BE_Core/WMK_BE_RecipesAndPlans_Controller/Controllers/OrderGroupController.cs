using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderGroupModel;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/order-group")]
	[ApiController]
	public class OrderGroupController : ControllerBase
	{
		private readonly IOrderGroupService _orderGroupService;
        public OrderGroupController(IOrderGroupService orderGroupService)
        {
            _orderGroupService = orderGroupService;
        }

		[HttpGet("get-all")]
		public async Task<IActionResult> GetAll()
		{
			var result = await _orderGroupService.GetAllAsync();
			return Ok(result);
		}

		[HttpGet("get-id")]
		public async Task<IActionResult> GetById([FromQuery] Guid orderGroupId)
		{
			var result = await _orderGroupService.GetOrderGroupByIdAsync(orderGroupId);
			return Ok(result);
		}

		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] CreateOrderGroupRequest model)
		{
			var result = await _orderGroupService.CreateOrderGroupAsync(model);
			return Ok(result);
		}

		[HttpPut("update")]
		public async Task<IActionResult> Update([FromBody] UpdateOrderGroupRequest model)
		{
			var result = await _orderGroupService.UpdateOrderGroupAsync(model);
			return Ok(result);
		}
		
		[HttpDelete("delete")]
		public async Task<IActionResult> Delete([FromQuery] IdOrderGroupRequest model)
		{
			var result = await _orderGroupService.DeleteOrderGroupAsync(model);
			return Ok(result);
		}

		[HttpPut("change-status")]
		public async Task<IActionResult> ChangeStatus([FromQuery] IdOrderGroupRequest model)
		{
			var result = await _orderGroupService.ChangeStatusOrderGroupAsync(model);
			return Ok(result);
		}


		[HttpPut("cluster")]
		public async Task<IActionResult> Cluster([FromBody] ClusterOrderGroupRequest model)
		{
			var result = await _orderGroupService.OrderGroupClusterAsync(model);
			return Ok(result);
		}
	}
}
