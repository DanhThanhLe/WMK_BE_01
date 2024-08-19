using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel
{
	public class UpdateWeeklyPlanRequestModel
	{
		public string? Title { get; set; } = string.Empty;
		public DateTime? BeginDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string? Description { set; get; } = string.Empty;
		public string? UrlImage { get; set; } = string.Empty;
		public List<RecipeWeeklyPlanCreate> recipeIds { get; set; }
	}
}
