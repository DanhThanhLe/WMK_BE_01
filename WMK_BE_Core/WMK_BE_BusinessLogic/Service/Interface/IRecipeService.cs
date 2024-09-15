using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeNutrientModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface IRecipeService
	{
		#region Get
		public Task<ResponseObject<List<RecipeResponse>>> GetAllRecipesAsync(string? userId , GetAllRecipesRequest? model);
		public Task<ResponseObject<RecipeResponse>> GetRecipeById(string id);
		public Task<ResponseObject<List<RecipeResponse>>> GetRecipesByNameStatusAsync(string name , bool status);
		public Task<ResponseObject<List<RecipeResponse>>> GetRecipesByNameAsync(string name);
		public Task<ResponseObject<List<RecipeResponse>>> GetListByCategoryId(Guid categoryId);
		#endregion
		
		public Task<ResponseObject<RecipeResponse>> CreateRecipeAsync(string createdBy , CreateRecipeRequest recipe);
		
		#region Update
		public Task<ResponseObject<RecipeResponse>> UpdateRecipeAsync(string updatedBy , Guid idRecipe , CreateRecipeRequest recipe);
		Task<ResponseObject<List<RecipeNutrientResponse>>> UpdateRecipeByIngredient(Guid ingredientId);
		public Task<bool> AutoUpdateRecipeAsync(Guid? recipeId);
		#endregion
		
		public Task<ResponseObject<RecipeResponse>> DeleteRecipeById(Guid userId , Guid request);//xoa khoi db
		
		#region Change status
		public Task<ResponseObject<RecipeResponse>> ChangeStatusProcessAsync(Guid id , ChangeRecipeStatusRequest recipe);
		public Task<ResponseObject<RecipeResponse>> ChangeBaseStatus(Guid id , ChangeRecipeBaseStatusRequest recipe);
		#endregion

	}
}
