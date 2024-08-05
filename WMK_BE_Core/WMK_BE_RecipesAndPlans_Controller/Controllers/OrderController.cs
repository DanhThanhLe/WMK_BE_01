using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderModel;
using WMK_BE_BusinessLogic.ResponseObject;
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
		#region Get
		//
		[HttpGet("get-all")]
		[Authorize(Roles = "Admin,Manager,Staff")]
		public async Task<IActionResult> GetAll([FromQuery] GetAllOrdersRequest? model)
		{
			var result = await _orderService.GetAllOrdersAsync(model);
			return StatusCode(result.StatusCode , result);
		}
		//
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
		//
		[HttpGet("get-order/{id}")]
		[Authorize]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _orderService.GetOrderByIdAsync(id);
			return StatusCode(result.StatusCode , result);
		}
		//
		[HttpGet("remove-all-ordergroup")]
		[Authorize]
		public async Task<IActionResult> RemoveFromOrdergroup()
		{
			var result = await _orderService.RemoveAllOrdersFromOrderGroupsAsync();
			return StatusCode(result.StatusCode , result);
		}
		#endregion
		#region Create
		//
		[HttpPost("create")]
		[Authorize]
		public async Task<IActionResult> Create([FromBody] CreateOrderRequest model)
		{
			var result = await _orderService.CreateOrderAsync(model);
			return StatusCode(result.StatusCode , result);
		}
		#endregion
		#region Update
		//
		[HttpPut("change-status/{id}")]
		[Authorize]
		public async Task<IActionResult> ChangeStatus(Guid id , [FromQuery] ChangeStatusOrderRequest model)
		{
			var result = await _orderService.ChangeStatusOrderAsync(id , model);
			return StatusCode(result.StatusCode , result);
		}
		//
		[HttpPut("change-ordergroup/{idOrder}")]
		[Authorize]
		public async Task<IActionResult> ChangeOrdergroup(Guid idOrder , [FromBody] Guid idOrderGroup)
		{
			var result = await _orderService.ChangeOrderGroupAsync(idOrder , idOrderGroup);
			return StatusCode(result.StatusCode , result);
		}
		#endregion
		#region Delete
		//
		[HttpDelete("delete/{id}")]
		[Authorize]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _orderService.DeleteOrderAsync(id);
			return StatusCode(result.StatusCode , result);
		}
		//
		[HttpDelete("remove-ordergroup/{idOrder}")]
		[Authorize]
		public async Task<IActionResult> RemoveFromOrdergroup(Guid idOrder)
		{
			var result = await _orderService.RemoveOrderFormOrderGroupAsync(idOrder);
			return StatusCode(result.StatusCode , result);
		}
		#endregion

	}
}
