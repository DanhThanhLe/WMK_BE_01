using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderGroupModel;
using WMK_BE_BusinessLogic.Service.Implement;
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
		//
		[HttpGet("get-all")]
		public async Task<IActionResult> GetAll([FromQuery] GetALLOrderGroupsRequest? model)
		{
			var result = await _orderGroupService.GetAllAsync(model);
			return StatusCode(result.StatusCode , result);
		}
		//
		[HttpGet("get-id/{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _orderGroupService.GetOrderGroupByIdAsync(id);
			return StatusCode(result.StatusCode , result);
		}
		//
		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] CreateOrderGroupRequest model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if ( userId == null )
			{
				return Unauthorized(new { Message = "Invalid token, user ID not found" });
			}
			string assignedBy = userId.ToString();
			var result = await _orderGroupService.CreateOrderGroupAsync(model , assignedBy);
			return StatusCode(result.StatusCode , result);
		}
		//
		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update([FromBody] UpdateOrderGroupRequest model , string id)
		{
			Guid ingredientId;
			if ( Guid.TryParse(id , out ingredientId) )
			{
				var result = await _orderGroupService.UpdateOrderGroupAsync(model , id);
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
		//
		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _orderGroupService.DeleteOrderGroupAsync(id);
			return StatusCode(result.StatusCode , result);
		}

		//
		[HttpPut("change-status/{id}")]
		public async Task<IActionResult> ChangeStatus(Guid id , ChangeStatusOrderGroupRequest model)
		{
			var result = await _orderGroupService.ChangeStatusOrderGroupAsync(id , model);
			return StatusCode(result.StatusCode , result);
		}

		//
		[HttpPut("cluster")]
		public async Task<IActionResult> Cluster()
		{
			var result = await _orderGroupService.OrderGroupClusterAsync();
			return StatusCode(result.StatusCode , result);
		}
	}
}
