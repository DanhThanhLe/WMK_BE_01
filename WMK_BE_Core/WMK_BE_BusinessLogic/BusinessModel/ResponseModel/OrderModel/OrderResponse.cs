using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderModel
{
	public class OrderResponse
	{
		public string Id { get; set; } = string.Empty;
		public string UserId { get; set; } = string.Empty;
		//public string? StanderdWeeklyPlanId { get; set; }

		public string Note { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public DateTime ShipDate { get; set; }
		public DateTime OrderDate { get; set; }
		public double TotalPrice { get; set; }
		public string Status { get; set; } = string.Empty;
	}
}
