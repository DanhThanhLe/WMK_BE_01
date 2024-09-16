using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.CustomPlanModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.FeedbackModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.TransactionModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.WeeklyPlanModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderModel
{
	public class OrderResponse
	{
		public Guid Id { get; set; }
		public int OrderCode { get; set; }
		public string UserId { get; set; } = string.Empty;
		public string OrderGroupId { get; set; } = string.Empty;
		//public string? StanderdWeeklyPlanId { get; set; }
		public string ReceiveName { get; set; } = string.Empty;
		public string ReceivePhone { get; set; } = string.Empty;
		public string Note { get; set; } = string.Empty;
		public string? Message { get; set; }
		public string Address { get; set; } = string.Empty;
		public double Longitude { get; set; }//kinh dộ
		public double Latitude { get; set; }//vĩ độ
		public string? Img { get; set; }
		public DateTime ShipDate { get; set; }
		public DateTime OrderDate { get; set; }
		public double TotalPrice { get; set; }
		public string Status { get; set; } = string.Empty;
		public WeeklyPlanResponseModel weeklyPlan { get; set; }
		public TransactionResponse Transaction { get; set; }
		//order detail
		public List<OrderDetailResponse> OrderDetails { get; set; }
		public FeedbackResponse FeedBacks { get; set; }


    }

	public class OrderResponseId
	{
		public string Id { get; set; }
	}
}
