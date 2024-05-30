using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
	public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
	{
        public CategoryRepository(WeMealKitContext context) : base(context)
        {
            
        }
		
		public async Task<bool> RecipeExistCategoryAsync(Guid id)
		{
			var recipeCategory = await _dbSet.Include(c => c.RecipeCategories).FirstOrDefaultAsync(c => c.Id == id);
			if(recipeCategory != null && recipeCategory.RecipeCategories.Count() > 0 )
			{
				return true;
			}
			return false;
		}
	}
}
