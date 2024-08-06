using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel
{
	public class CreateWeeklyPlanRequest
	{
		public string Title { get; set; } = string.Empty;
		public string? Description { set; get; } = string.Empty;
        public string? UrlImage { get; set; } 
        public DateTime BeginDate { get; set; }//chua co validate
        public DateTime EndDate { get; set; }//chua co validate
        public List<RecipeWeeklyPlanCreate> recipeIds { get; set; }
	}
	public class CreateWeeklyPlanForCustomerRequest
	{
		public string Title { get; set; } = string.Empty;
		public string? Description { set; get; } = string.Empty;
		public string? WeeklyPlanId { set; get; } //cai nay chi dung de lay thong tin image cho img ben duoi
		public string? CreatedBy { get; set; } = string.Empty;
		public string? UrlImage { get; set; }
		public ProcessStatus? ProcessStatus { get; set; }
		public DateTime BeginDate { get; set; }//chua co validate
		public DateTime EndDate { get; set; }//chua co validate
		public List<RecipeWeeklyPlanCreate> recipeIds { get; set; }
	}
	public class RecipeWeeklyPlanCreate
	{
		public Guid recipeId { get; set; }
		public int Quantity { get; set; }
        public DayInWeek DayInWeek { get; set; }
        public MealInDay MealInDay { get; set; }

    }
}
