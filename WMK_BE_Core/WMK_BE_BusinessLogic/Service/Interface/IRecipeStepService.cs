using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeStepModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeStepModel.RecipeStep;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeStepModel.RecipeStep;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Service.Interface
{
    public interface IRecipeStepService
    {
        Task<ResponseObject<List<RecipeStepRespone>>> GetRecipeSteps();
        Task<ResponseObject<List<RecipeStepRespone>>> GetByRecipeId(Guid recipeId);
        Task<ResponseObject<List<RecipeStep>>> CreateRecipeSteps(Guid recipeId, List<CreateRecipeStepRequest> stepList);
        Task<ResponseObject<List<RecipeStep>>> DeleteRecipeStepsByRecipe(Guid recipeId);
        Task<ResponseObject<RecipeStep>> DeleteAsync(Guid recipeStepId);
        Task<ResponseObject<List<RecipeStep>>> UpdateRecipeStepsByRecipe(Guid Id, CreateRecipeStepRequest model);
    }
}
