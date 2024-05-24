using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel
{
	public class UpdateOrderRequest
	{
		public string Id { get; set; } = string.Empty;
		public string? StanderdWeeklyPlanId { get; set; }
		public string? Note { get; set; } = string.Empty;
		public string? Address { get; set; } = string.Empty;
		public DateTime? ShipDate { get; set; }
		public double? TotalPrice { get; set; }

	}
}
