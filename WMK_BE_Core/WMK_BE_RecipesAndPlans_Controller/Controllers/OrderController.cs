using Accord.Math;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Implement;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/order")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;
		private readonly ISendMailService _sendMailService;

		public OrderController(IOrderService orderService , ISendMailService sendMailService)
		{
			_orderService = orderService;
			_sendMailService = sendMailService;
		}
		#region Get
		[HttpGet("get-all")]
		[Authorize(Roles = "Admin,Manager,Staff")]
		public async Task<IActionResult> GetAll([FromQuery] GetAllOrdersRequest? model)
		{
			var result = await _orderService.GetAllOrdersAsync(model);
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
		[Authorize]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _orderService.GetOrderByIdAsync(id);
			return StatusCode(result.StatusCode , result);
		}
		#endregion

		#region Create
		[HttpPost("create")]
		[Authorize]
		public async Task<IActionResult> Create([FromBody] CreateOrderRequest model)
		{
			// Lấy chuỗi JSON từ token với Claim có tên "User"
			var userClaim = User.Claims.FirstOrDefault(c => c.Type == "User")?.Value;

			if ( string.IsNullOrEmpty(userClaim) )
			{
				return Unauthorized("User not found in token");
			}
			// Chuyển chuỗi JSON thành đối tượng User
			var user = JsonConvert.DeserializeObject<User>(userClaim);

			if ( user == null )
			{
				return Unauthorized("Invalid user data");
			}
			var result = await _orderService.CreateOrderAsync(model);
			if ( result.Data != null )
			{
				StringBuilder emailContent = new StringBuilder();

				// Tạo nội dung email dạng HTML
				emailContent.Append("<p>Dear " + result.Data.ReceiveName + ",</p>");
				emailContent.Append("<p>Your order on WeMealKit has been successfully created.</p>");
				emailContent.Append("<p>Order Information:</p>");
				emailContent.Append("<p>----------------------------------------------------</p>");
				emailContent.Append("<p>Order Code: " + result.Data.OrderCode + "</p>");
				emailContent.Append("<p>Order Date: " + result.Data.OrderDate.ToString("dd/MM/yyyy") + "</p>");
				emailContent.Append("<p>Ship Date: " + result.Data.ShipDate.ToString("dd/MM/yyyy") + "</p>");
				emailContent.Append("<p>Recipient Name: " + result.Data.ReceiveName + "</p>");
				emailContent.Append("<p>Recipient Phone: " + result.Data.ReceivePhone + "</p>");
				emailContent.Append("<p>Address: " + result.Data.Address + "</p>");
				emailContent.Append("<p>Total Price: $" + result.Data.TotalPrice.ToString("N2") + "</p>");
				emailContent.Append("<br/><p>Thank you for shopping with us!</p>");
				emailContent.Append("<p>----------------------------------------------------</p>");

				// Gửi email với nội dung HTML
				_sendMailService.SendMail(user.Email , "Order created on WeMealKit" , emailContent.ToString());

			}
			return StatusCode(result.StatusCode , result);
		}
		#endregion

		#region Update
		[HttpPut("change-status/{id}")]
		[Authorize]
		public async Task<IActionResult> ChangeStatus(Guid id , [FromQuery] ChangeStatusOrderRequest model)
		{
			var result = await _orderService.ChangeStatusOrderAsync(id , model);
			//mỗi lần thay đổi sẻ gửi mail về cho customer
			if ( result.Data != null && result.Data.UserId != null )
			{
				_sendMailService.SendMail(result.Data.UserId , "Order status information on WeMealKit" , "Your order on WemealKit with code ("
										+ result.Data.OrderCode + ") has been update to ("
										+ result.Data.Status + ").");
			}
			return StatusCode(result.StatusCode , result);
		}

		[HttpPut("change-ordergroup/{idOrder}")]
		[Authorize]
		public async Task<IActionResult> ChangeOrdergroup(Guid idOrder , [FromBody] Guid idOrderGroup)
		{
			var result = await _orderService.ChangeOrderGroupAsync(idOrder , idOrderGroup);
			return StatusCode(result.StatusCode , result);
		}
		#endregion

		#region Delete
		[HttpDelete("delete/{id}")]
		[Authorize]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _orderService.DeleteOrderAsync(id);
			return StatusCode(result.StatusCode , result);
		}

		[HttpDelete("remove-ordergroup/{idOrder}")]
		[Authorize]
		public async Task<IActionResult> RemoveFromOrdergroup(Guid idOrder)
		{
			var result = await _orderService.RemoveOrderFormOrderGroupAsync(idOrder);
			return StatusCode(result.StatusCode , result);
		}

		[HttpDelete("reset-all-ordergroup")]
		[Authorize]
		public async Task<IActionResult> RemoveFromOrdergroup()
		{
			var result = await _orderService.RemoveAllOrdersFromOrderGroupsAsync();
			return StatusCode(result.StatusCode , result);
		}
        #endregion
        [HttpPut("change-status-test/{id}")]
        [Authorize]
        public async Task<IActionResult> ChangeStatusTest(Guid id, [FromQuery] ChangeStatusOrderRequest model)
        {
            var result = await _orderService.TestChangeStatus(id, model);
            //mỗi lần thay đổi sẻ gửi mail về cho customer
            if (result.Data != null && result.Data.UserId != null)
            {
                _sendMailService.SendMail(result.Data.UserId, "Order status information on WeMealKit", "Your order on WemealKit with code ("
                                        + result.Data.OrderCode + ") has been update to ("
                                        + result.Data.Status + ").");
            }
            return StatusCode(result.StatusCode, result);
        }


    }
}
