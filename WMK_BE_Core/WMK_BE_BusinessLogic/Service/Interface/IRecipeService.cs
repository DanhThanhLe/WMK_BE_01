using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
    public interface IRecipeService
    {
        public Task<ResponseObject<RecipeResponse>> GetRecipes();

        public Task<ResponseObject<RecipeResponse>> GetRecipeById(string id);

        public Task<ResponseObject<RecipeResponse>> GetRecipeByName(string name);

        //public Task<ResponseObject<RecipeResponse>> CreateRecipe(CreateRecipeRequest recipe);

        //public Task<ResponseObject<RecipeResponse>> UpdateRecipe(RecipeRequest recipe);

        public Task<ResponseObject<RecipeResponse>> DeleteRecipeById(IdRecipeRequest recipe);//xoa khoi db
        //public Task<ResponseObject<RecipeResponse>> RemoveRecipe(Guid id);//an khoi app
        public Task<ResponseObject<RecipeResponse>> ChangeStatus(ChangeRecipeStatusRequest recipe);

        //public Task<ResponseObject<RecipeResponse>> FilterToMenu(MenuFilterRequest request);

        //public Task<ResponseObject<RecipeResponse>> GetRecipesByEquipmentId(int equipmentId);

        //public Task<ResponseObject<RecipeResponse>> GetRecipesByHealthConditionId(int healthConditionId);

        //public Task<ResponseObject<RecipeResponse>> GetRecipesByCookingHobbyId(int cookingHobbyId);

        ////public Task<ResponseObject<RecipeResponse>> GetRecipesWithListAndMeal(List<Recipe> list, string meal);

        //public Task<ResponseObject<RecipeResponse>> GetRecipesWithMeal(string meal);

        //public Task<ResponseObject<RecipeResponse>> GetIngredientAndAmountByRecipeId(int recipeId);

        //public Task<ResponseObject<RecipeResponse>> GetRecipeWithIngredientsList(List<int> ingredientsInputList);
    }
}
