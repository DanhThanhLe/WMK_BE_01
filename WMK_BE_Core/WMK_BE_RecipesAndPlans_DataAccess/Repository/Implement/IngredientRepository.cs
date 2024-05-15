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
    public class IngredientRepository : BaseRepository<Ingredient>, IIngredientRepository
    {
        public IngredientRepository(DbContext context) : base(context) { }

        public async Task<bool> ChangeStatusAsync(Guid id, BaseStatus status)
        {
            var ingredient = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
            if (ingredient != null)
            {
                ingredient.Status = status;
                _dbSet.Update(ingredient);
                return true;
            }
            return false;
        }
    }
}
