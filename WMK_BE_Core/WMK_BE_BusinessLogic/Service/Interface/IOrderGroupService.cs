using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderGroupModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderGroupModel;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface IOrderGroupService
	{

		Task<ResponseObject<List<OrderGroupsResponse>>> GetAllAsync(GetALLOrderGroupsRequest? model);
		Task<ResponseObject<OrderGroupsResponse?>> GetOrderGroupByIdAsync(Guid orderGroupId);
		Task<ResponseObject<OrderGroupsResponse>> CreateOrderGroupAsync(CreateOrderGroupRequest model, string assignedBy);
		Task<ResponseObject<List<OrderGroupsResponse>>> OrderGroupClusterAsync();
		Task<ResponseObject<OrderGroupsResponse>> UpdateOrderGroupAsync(UpdateOrderGroupRequest model, string id);
		Task<ResponseObject<OrderGroupsResponse>> DeleteOrderGroupAsync(Guid id);
		Task<ResponseObject<OrderGroupsResponse>> ChangeStatusOrderGroupAsync(Guid id, ChangeStatusOrderGroupRequest model);

	}
}
