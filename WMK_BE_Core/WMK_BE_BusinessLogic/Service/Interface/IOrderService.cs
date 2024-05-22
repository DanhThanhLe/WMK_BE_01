using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderModel;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface IOrderService
	{
		Task<ResponseObject<List<OrderResponse>>> GetAllOrders();
		Task<ResponseObject<OrderResponse?>> GetOrderByIdAsync(Guid id);
		Task<ResponseObject<BaseOrderResponse>> CreateOrderAsync(CreateOrderRequest model);
		Task<ResponseObject<BaseOrderResponse>> UpdateOrderAsync(UpdateOrderRequest model);

	}
}
