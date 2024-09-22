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
		#region Get
		Task<ResponseObject<List<OrderResponse>>> GetAllOrdersAsync(GetAllOrdersRequest? model);
		Task<ResponseObject<OrderResponse?>> GetOrderByIdAsync(Guid id);
		Task<ResponseObject<List<OrderResponse>>> GetOrdersByUserId(Guid userId);
		#endregion

		Task<ResponseObject<OrderResponse>> CreateOrderAsync(CreateOrderRequest model);

		#region Update
		Task<ResponseObject<OrderResponse>> UpdateOrderAsync(string id , UpdateOrderRequest model);
		Task<ResponseObject<OrderResponse>> UpdateOrderByUserAsync(UpdateOrderByUserRequest model);
		#endregion

		#region Delete
		Task<ResponseObject<OrderResponse>> DeleteOrderAsync(Guid id);
		Task<ResponseObject<OrderResponse>> RemoveOrderFormOrderGroupAsync(Guid idOrder);
		Task<ResponseObject<List<OrderResponse>>> RemoveAllOrdersFromOrderGroupsAsync();
		#endregion

		#region Change
		Task<ResponseObject<OrderResponse>> ChangeOrderGroupAsync(Guid idOrder , Guid idOrderGroup);
		Task<ResponseObject<OrderResponse>> ChangeStatusOrderAsync(Guid id , ChangeStatusOrderRequest model);

		Task<ResponseObject<OrderResponse>> TestChangeStatus(Guid id, ChangeStatusOrderRequest model);
        #endregion


    }
}
