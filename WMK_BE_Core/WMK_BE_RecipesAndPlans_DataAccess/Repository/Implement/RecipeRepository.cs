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
    public class RecipeRepository : BaseRepository<Recipe>, IRecipeRepository
    {
        public RecipeRepository(WeMealKitContext context) : base(context)
        { }



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

        public override async Task<List<Recipe>> GetAllAsync()
        {
            var recipes = await _dbSet
                                .Include(r => r.RecipeIngredients)
                                    .ThenInclude(ri => ri.Ingredient)
                                        .ThenInclude(i => i.IngredientNutrient)
                                .Include(r => r.RecipeIngredients)
                                    .ThenInclude(ri => ri.Ingredient)
                                        .ThenInclude(i => i.IngredientCategory)
                                .Include(r => r.RecipeCategories)
                                    .ThenInclude(rc => rc.Category)
                                .Include(r => r.RecipeNutrient)
                                .Include(r => r.RecipeSteps)
                                .ToListAsync();
            return recipes;
        }

        public override async Task<Recipe> GetByIdAsync(string id)
        {
            try
            {
                Guid guidId;
                if (!Guid.TryParse(id, out guidId))
                {
                    return null;
                }

                var recipe = await _dbSet
                                .Include(r => r.RecipeIngredients)
                                    .ThenInclude(ri => ri.Ingredient)
                                        .ThenInclude(i => i.IngredientNutrient)
                                .Include(r => r.RecipeIngredients)
                                    .ThenInclude(ri => ri.Ingredient)
                                        .ThenInclude(i => i.IngredientCategory)
                                .Include(r => r.RecipeCategories)
                                    .ThenInclude(rc => rc.Category)
                                .Include(r => r.RecipeNutrient)
                                .Include(r => r.RecipeSteps)
                                .FirstOrDefaultAsync(r => r.Id == guidId);

                return recipe;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred in GetByIdAsync: {ex}");
                return null;
            }
        }

        public override IQueryable<Recipe> Get(Expression<Func<Recipe, bool>> expression)
        {
            return _dbSet
                .Where(expression)
                .Include(r => r.RecipeIngredients)
                                    .ThenInclude(ri => ri.Ingredient)
                                        .ThenInclude(i => i.IngredientNutrient)
                .Include(r => r.RecipeIngredients)
                                    .ThenInclude(ri => ri.Ingredient)
                                        .ThenInclude(i => i.IngredientCategory)
                .Include(r => r.RecipeCategories)
                .Include(r => r.RecipeNutrient)
                .Include(r => r.RecipeSteps);
        }
    }
}
