﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
	public class RecipeIngredientRepository : BaseRepository<RecipeIngredient>, IRecipeIngredientRepository
	{
        public RecipeIngredientRepository(WeMealKitContext context) : base(context)
        {
            
        }

		public async Task AddRangeAsync(IEnumerable<RecipeIngredient> recipeAmounts)
		{
			await _dbSet.AddRangeAsync(recipeAmounts);
		}

		public void DeleteRange(IEnumerable<RecipeIngredient> recipeIngredients)
		{
			_dbSet.RemoveRange(recipeIngredients);
		}
	}
}
