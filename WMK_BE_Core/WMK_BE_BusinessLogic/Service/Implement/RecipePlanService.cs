using WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class RecipePlanService : IRecipePlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecipePlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseObject<List<RecipePLan>?>> CreateRecipePlanAsync(Guid weeklyPlanId, List<RecipeWeeklyPlanCreate> recipeIds)
        {
            var result = new ResponseObject<List<RecipePLan>?>();
            var recipePlans = new List<RecipePLan>();
            try
            {
                //check weeklyPlan have exist
                var weeklyPlanExist = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(weeklyPlanId.ToString());
                if (weeklyPlanExist != null && (weeklyPlanExist.ProcessStatus == ProcessStatus.Processing || weeklyPlanExist.ProcessStatus == ProcessStatus.Customer))//ko phai status la processing hoac la cua customer thi ko cho tao
                {
                    foreach (var recipe in recipeIds)
                    {
                        var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipe.recipeId.ToString());
                        if (recipeExist != null && recipeExist.ProcessStatus == ProcessStatus.Approved)
                        {
                            var recipePlan = new RecipePLan
                            {
                                StandardWeeklyPlanId = weeklyPlanId,
                                RecipeId = recipeExist.Id,
                                Recipe = recipeExist,
                                WeeklyPlan = weeklyPlanExist,
                                Quantity = recipe.Quantity,
                                Price = recipeExist.Price * recipe.Quantity,
                                DayInWeek = recipe.DayInWeek,
                                MealInDay = recipe.MealInDay,
                            };
                            recipePlans.Add(recipePlan);
                        }
                        else 
                        {
                            result.StatusCode = 404;
                            result.Message = $"Recipe with ID {recipeExist.Id} not found!";
                            return result;
                        }
                    }
                    //add recipe plan into DB
                    if (recipePlans.Any())
                    {
                        await _unitOfWork.RecipePlanRepository.AddRangeAsync(recipePlans);
                    }
                    //add list recipe plan into DB
                    await _unitOfWork.CompleteAsync();
                    result.StatusCode = 200;
                    result.Message = "Create recipe plan successfully.";
                    result.Data = recipePlans;
                    return result;
                }
                else //trg hop khac
                {
                    result.StatusCode = 404;
                    result.Message = "Weekly plan not exist!";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.StatusCode = 500;
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<ResponseObject<List<RecipePLan>?>> UpdateRecipePlanAsync(Guid weeklyPlanId, List<Guid> newRecipesId)
        {
            var result = new ResponseObject<List<RecipePLan>?>();
            try
            {
                // Check if weeklyPlan exists
                var weeklyPlan = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(weeklyPlanId.ToString());
                if (weeklyPlan == null)
                {
                    result.StatusCode = 404;
                    result.Message = "Weekly plan does not exist!";
                    return result;
                }

                // Get existing recipe plans
                var existingRecipePlans = await _unitOfWork.RecipePlanRepository.GetListByPlanIdAsync(weeklyPlanId);

                // Find recipes to remove by newRecipeId
                var recipesToRemove = existingRecipePlans
                    .Where(rp => !newRecipesId.Contains(rp.RecipeId))
                    .ToList();

                // Remove old recipes
                if (recipesToRemove.Any())
                {
                    _unitOfWork.RecipePlanRepository.RemoveRange(recipesToRemove);
                }

                // Find recipes to add
                var existingRecipeIds = existingRecipePlans.Select(rp => rp.RecipeId).ToList();
                var recipesToAddIds = newRecipesId
                    .Where(id => !existingRecipeIds.Contains(id))
                    .ToList();

                // Add new recipes
                var newRecipePlans = new List<RecipePLan>();
                foreach (var recipeId in recipesToAddIds)
                {
                    var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
                    if (recipe != null)
                    {
                        var recipePlan = new RecipePLan
                        {
                            StandardWeeklyPlanId = weeklyPlanId,
                            RecipeId = recipeId,
                            Recipe = recipe,
                            WeeklyPlan = weeklyPlan
                        };
                        newRecipePlans.Add(recipePlan);
                    }
                    else
                    {
                        result.StatusCode = 404;
                        result.Message = $"Recipe with ID {recipeId} not found!";
                        return result;
                    }
                }

                // Save changes to database
                if (newRecipePlans.Any())
                {
                    await _unitOfWork.RecipePlanRepository.AddRangeAsync(newRecipePlans);
                }

                await _unitOfWork.CompleteAsync();

                // Return success response
                result.StatusCode = 200;
                result.Message = "Recipe plans updated successfully.";
                result.Data = newRecipePlans;
                return result;
            }
            catch (Exception ex)
            {
                result.StatusCode = 500;
                result.Message = ex.Message;
                return result;
            }
        }

        //public async Task<ResponseObject<RecipePLan>> DeleteRecipePlan(Guid weeklyPlanId)
        //{

        //}//dung de xoa

        
    }
}
