using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
    public class RecipeCategoryRepository :BaseRepository<RecipeCategory>, IRecipeCategoryRepository
    {
        public RecipeCategoryRepository(WeMealKitContext context):base(context)
        {
            
        }
        public async Task AddRangeAsync(IEnumerable<RecipeCategory> recipeCategories)
        {
            await _dbSet.AddRangeAsync(recipeCategories);
        }
    }
}
