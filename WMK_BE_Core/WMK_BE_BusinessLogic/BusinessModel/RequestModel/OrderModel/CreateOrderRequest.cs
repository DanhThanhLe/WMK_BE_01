using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel
{
	public class CreateOrderRequest
	{
		public string UserId { get; set; } = string.Empty;
		public string? StanderdWeeklyPlanId { get; set; }
		public string? Note { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
        public double TotalPrice { get; set; }
		public double Longitude { get; set; }//kinh dộ
		public double Latitude { get; set; }//vĩ độ
		public List<CreateCustomPlanRequest>? RecipeList { get; set; }
	}
	public class CreateCustomPlanRequest
	{
		public Guid RecipeId { get; set; }
		public Guid? StandardWeeklyPlanId { get; set; }
        public DayInWeek DayInWeek { get; set; }
        public MealInDay MealInDay { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
