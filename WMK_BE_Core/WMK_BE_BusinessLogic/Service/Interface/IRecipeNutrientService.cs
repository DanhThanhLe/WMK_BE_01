using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.NutritionModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Service.Interface
{
    public interface IRecipeNutrientService
    {
        Task<ResponseObject<RecipeNutrient>> CreateRecipeNutrientAsync(Guid recipeId);
		
        public Task<bool> AutoUpdateNutrientByRecipe(Guid recipeId);

	}
}
