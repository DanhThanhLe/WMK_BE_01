using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface IRecipePlanService
	{
		Task<ResponseObject<List<RecipePLan>?>> CreateRecipePlanAsync(Guid weeklyPlanId, List<RecipeWeeklyPlanCreate> recipesId);
		Task<ResponseObject<List<RecipePLan>?>> UpdateRecipePlanAsync(Guid weeklyPlanId , List<Guid> newRecipesId);
	}
}
