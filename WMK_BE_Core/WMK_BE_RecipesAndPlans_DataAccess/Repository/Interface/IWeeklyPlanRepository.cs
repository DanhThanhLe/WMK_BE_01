using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface
{
	public interface IWeeklyPlanRepository: IBaseRepository<WeeklyPlan>
	{
		Task<bool> RecipeExistInWeeklyPlanAsync(Guid weeklyPlanId);
		Task<List<WeeklyPlan>> GetAllWeeklyPlanFilterAsync(DateTime startOfWeek , DateTime enOfWeek);
		void RemoveRange(IEnumerable<WeeklyPlan> weeklyPlans);

	}
}
