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
		public string? Description { set; get; } = string.Empty;
		public string CreatedBy { get; set; } = string.Empty;
		public List<RecipeWeeklyPlanCreate> recipeIds { get; set; }
	}
	public class RecipeWeeklyPlanCreate
	{
		public Guid recipeId { get; set; }
        public int Amount { get; set; }
    }
}
