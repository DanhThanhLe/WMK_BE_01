using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class RecipePlanService : IRecipePlanService
	{
		private readonly IUnitOfWork _unitOfWork;

        public RecipePlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

		public async Task<ResponseObject<List<RecipePLan>?>> CreateRecipePlanAsync(Guid weeklyPlanId , List<Guid> recipesId)
		{
			var result = new ResponseObject<List<RecipePLan>?>();
			var recipePlans = new List<RecipePLan>();
			try
			{
				//check weeklyPlan have exist
				var weeklyPlan = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(weeklyPlanId.ToString());
				if(weeklyPlan == null)
				{
					result.StatusCode = 404;
					result.Message = "weekly plan not exist!";
					return result;
				}
				foreach(var recipeId in recipesId)
				{
					var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
					if(recipe != null)
					{
						//if customer create then increase popularity 
						if(weeklyPlan.ProcessStatus == WMK_BE_RecipesAndPlans_DataAccess.Enums.ProcessStatus.Customer)
						recipe.Popularity++;
						var recipePlan = new RecipePLan
						{
							PlanId = weeklyPlanId,
							RecipeId = recipeId,
							Recipe = recipe,
							WeeklyPlan = weeklyPlan
						};
						recipePlans.Add(recipePlan);
					}
					else
					{
						result.StatusCode = 404;
						result.Message = $"Recipe with ID {recipeId} not found!";
						return result;
					}
					//add recipe plan into DB
					await _unitOfWork.RecipePlanRepository.AddRangeAsync(recipePlans);//add list recipe plan into DB
					await _unitOfWork.CompleteAsync();
				}
				result.StatusCode = 200;
				result.Message = "Create recipe plan successfully.";
				result.Data = recipePlans;
				return result;
			}
			catch (Exception ex)
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
				return result;
			}
		}
	}
}
