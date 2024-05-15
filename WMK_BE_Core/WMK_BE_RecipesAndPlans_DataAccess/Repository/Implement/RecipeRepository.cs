using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
	public class RecipeRepository : BaseRepository<Recipe>, IRecipeRepository
    {
        public RecipeRepository(RecipesAndPlansContext context) : base(context)
        {}

        public async Task<bool> ChangeStatusAsync(Guid id, ProcessStatus status)
        {
            var recipe = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
            if (recipe != null)
            {
                recipe.ProcessStatus = status;
                _dbSet.Update(recipe);
                return true;
            }
            return false;
        }
    }
}
