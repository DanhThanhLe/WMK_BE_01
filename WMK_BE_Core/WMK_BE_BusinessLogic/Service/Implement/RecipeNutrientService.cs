using AutoMapper;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.NutritionModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeNutrientModel;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class RecipeNutrientService : IRecipeNutrientService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public RecipeNutrientService(IUnitOfWork unitOfWork , IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ResponseObject<RecipeNutrient>> CreateRecipeNutrientAsync(Guid recipeId)
		{
			var result = new ResponseObject<RecipeNutrient>();//result
			var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());//tim recipe
			if ( recipeExist != null )
			{
				if ( recipeExist.RecipeNutrient == null )
				{
					//create recipe nutrient
					var newRecipeNutrient = new RecipeNutrient
					{
						RecipeID = recipeId ,
					};
					var createResult = await _unitOfWork.RecipeNutrientRepository.CreateAsync(newRecipeNutrient);
					if ( !createResult )
					{
						result.StatusCode = 500;
						result.Message = "Create recipe nutrient unsuccess!";
						return result;
					}
					//success
					await _unitOfWork.CompleteAsync();
					newRecipeNutrient.Recipe = recipeExist;
					recipeExist.RecipeNutrient = newRecipeNutrient;
					await _unitOfWork.CompleteAsync();
				}
				//call update recipeNutrient
				var updateRecipeNutrient = await AutoUpdateNutrientByRecipe(recipeId);
				if ( updateRecipeNutrient )
				{
					result.StatusCode = 200;
					result.Message = "Create recipe nutrient success!";
					return result;
				}
				result.StatusCode = 500;
				result.Message = "Update recipe nutrient unsuccess!";
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Recipe not exist!";
			return result;

		}

		public async Task<bool> AutoUpdateNutrientByRecipe(Guid recipeId)
		{
			var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
			if ( recipeExist == null )
			{
				return false;
			}
			//xử lý update nutrient của recipe theo từng ingredient
			foreach ( var recipeIngredient in recipeExist.RecipeIngredients )
			{
				var ingredient = await _unitOfWork.IngredientRepository.GetByIdAsync(recipeIngredient.IngredientId.ToString());
				if ( ingredient != null )
				{
					recipeExist.RecipeNutrient.Sodium += ingredient.IngredientNutrient.Sodium;
					recipeExist.RecipeNutrient.Sugar += ingredient.IngredientNutrient.Sugar;
					recipeExist.RecipeNutrient.DietaryFiber += ingredient.IngredientNutrient.DietaryFiber;
					recipeExist.RecipeNutrient.Calories += ingredient.IngredientNutrient.Calories;
					recipeExist.RecipeNutrient.SaturatedFat += ingredient.IngredientNutrient.SaturatedFat;
					recipeExist.RecipeNutrient.Carbonhydrate += ingredient.IngredientNutrient.Carbonhydrate;
					recipeExist.RecipeNutrient.Fat += ingredient.IngredientNutrient.Fat;
					recipeExist.RecipeNutrient.Protein += ingredient.IngredientNutrient.Protein;
				}
			}
			await _unitOfWork.CompleteAsync();
			return true;
		}
	}
}
