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
	public interface IRecipeIngredientService
	{
		#region Get
		Task<ResponseObject<List<RecipeIngredientResponse>>> GetAll();
		Task<ResponseObject<List<RecipeIngredientResponse>>> GetListByRecipeId(Guid recipeId);
		#endregion

		Task<ResponseObject<List<RecipeIngredient>?>> CreateRecipeIngredientAsync(Guid recipeId , List<CreateRecipeIngredientRequest> recipeIngredientRequest);

		Task<ResponseObject<List<RecipeIngredient>?>> DeleteRecipeIngredientByRecipeAsync(Guid recipeId);

	}
}
