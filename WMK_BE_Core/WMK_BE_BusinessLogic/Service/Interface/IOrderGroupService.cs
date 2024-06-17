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

		Task<ResponseObject<List<OrderGroupsResponse>>> GetAllAsync();
		Task<ResponseObject<OrderGroupsResponse?>> GetOrderGroupByIdAsync(Guid orderGroupId);
		Task<ResponseObject<OrderGroupsResponse>> CreateOrderGroupAsync(CreateOrderGroupRequest model);
		Task<ResponseObject<List<OrderGroupsResponse>>> OrderGroupClusterAsync(ClusterOrderGroupRequest model);
		Task<ResponseObject<OrderGroupsResponse>> UpdateOrderGroupAsync(UpdateOrderGroupRequest model);
		Task<ResponseObject<OrderGroupsResponse>> DeleteOrderGroupAsync(IdOrderGroupRequest model);
		Task<ResponseObject<OrderGroupsResponse>> ChangeStatusOrderGroupAsync(IdOrderGroupRequest model);

	}
}
