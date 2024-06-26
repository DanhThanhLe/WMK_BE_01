﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeAmountModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface IRecipeAmountService
	{

		Task<ResponseObject<List<RecipeIngredient>?>> CreateRecipeAmountAsync(Guid recipeId , List<RecipeAmountCreateModel> ingredientModels);
		Task<ResponseObject<RecipeAmountResponse>> GetAll();
		Task<ResponseObject<RecipeAmountResponse>> GetListByRecipeId(Guid recipeId);

    }
}
