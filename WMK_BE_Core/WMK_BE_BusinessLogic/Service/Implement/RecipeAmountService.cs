using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class RecipeAmountService : IRecipeAmountService
	{
		private readonly IUnitOfWork _unitOfWork;

		public RecipeAmountService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public async Task<ResponseObject<List<RecipeAmount>?>> CreateRecipeAmountAsync(Guid recipeId , List<RecipeAmountModel> ingredientModels)
		{
			var result = new ResponseObject<List<RecipeAmount>?>();
			var recipeAmounts = new List<RecipeAmount>();
			try
			{
				//check recipe exist
				var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());

				if ( recipeExist == null )
				{
					result.StatusCode = 404;
					result.Message = "Recipe not exist!";
					return result;
				}
				//scan list ingredient
				foreach ( var ingredient in ingredientModels )
				{
					var ingredientEixst = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredient.IngredientId.ToString());
					if ( ingredientEixst != null )
					{
						var recipeAmount = new RecipeAmount
						{
							Ingredient = ingredientEixst ,
							IngredientId = ingredient.IngredientId ,
							RecipeId = recipeId ,
							Recipe = recipeExist ,
							Amount = ingredient.amount,
						};
						//await _unitOfWork.RecipeAmountRepository.CreateAsync(recipeAmount);
						//await _unitOfWork.CompleteAsync();
						recipeAmounts.Add(recipeAmount);
					}
					else
					{
						result.StatusCode = 404;
						result.Message = $"Ingredient with ID {ingredient.IngredientId} not found!";
						return result;
					}
				}
				if ( recipeAmounts.Any() )
				{
					await _unitOfWork.RecipeAmountRepository.AddRangeAsync(recipeAmounts);
				}
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Create recipe amount successfully.";
				result.Data = recipeAmounts;
				return result;
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
				return result;
			}
		}
	}
}
