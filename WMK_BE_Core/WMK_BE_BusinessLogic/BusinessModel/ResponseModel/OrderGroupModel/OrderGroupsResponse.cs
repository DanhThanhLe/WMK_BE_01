using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderGroupModel
{
	public class OrderGroupsResponse
	{
		public Guid Id { get; set; }
		public Guid ShipperId { get; set; }
		public string ShipperUserName { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
        public DateTime AsignAt { get; set; }
		public string AsignBy { get; set; } = string.Empty;
		public BaseStatus Status { get; set; }

		public List<OrderResponse> Orders {  get; set; } 

	}
}
