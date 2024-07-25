using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface IOrderService
	{
		Task<ResponseObject<List<OrderResponse>>> GetAllOrders();
		Task<ResponseObject<List<OrderResponse>>> GetOrdersByUserId(Guid userId);

        Task<ResponseObject<OrderResponse?>> GetOrderByIdAsync(IdOrderRequest model);
		Task<ResponseObject<Guid>> CreateOrderAsync(CreateOrderRequest model);
		Task<ResponseObject<OrderResponse>> UpdateOrderAsync(UpdateOrderRequest model);
		Task<ResponseObject<OrderResponse>> UpdateOrderByUserAsync(UpdateOrderByUserRequest model);
		Task<ResponseObject<OrderResponse>> DeleteOrderAsync(IdOrderRequest model);
		Task<ResponseObject<OrderResponse>> ChangeStatusOrderAsync(ChangeStatusOrderRequest model);

	}
}
