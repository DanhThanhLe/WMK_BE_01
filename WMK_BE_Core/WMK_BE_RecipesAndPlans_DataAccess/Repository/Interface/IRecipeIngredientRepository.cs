﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface
{
	public interface IRecipeIngredientRepository : IBaseRepository<RecipeIngredient>
	{
		Task AddRangeAsync(IEnumerable<RecipeIngredient> recipeAmounts);
		public void DeleteRange(IEnumerable<RecipeIngredient> recipeIngredients);

	}
}
