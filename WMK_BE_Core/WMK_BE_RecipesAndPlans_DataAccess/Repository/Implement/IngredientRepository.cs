using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
		public override async Task<List<Ingredient>> GetAllAsync()
		{
            var ingredient = await _dbSet.Include(i => i.IngredientNutrient).Include(i => i.IngredientCategory).ToListAsync();
			return ingredient;
		}

        public override async Task<Ingredient> GetByIdAsync(string id)
        {
            try
            {
                Guid guidId;
                if (!Guid.TryParse(id, out guidId))
                {
                    return null;
                }

                var ingredient = await _dbSet
                                    .Include(i => i.IngredientNutrient)
                                    .Include(i => i.IngredientCategory)
                                    .FirstOrDefaultAsync(i => i.Id == guidId);

                return ingredient;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred in GetByIdAsync: {ex}");
                return null;
            }
        }

        public override IQueryable<Ingredient> Get(Expression<Func<Ingredient, bool>> expression)
        {
            return _dbSet
                .Where(expression)
                .Include(i => i.IngredientCategory)
                .Include(i => i.IngredientNutrient);
        }

    }
}
