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
	public class WeeklyPlanRepository : BaseRepository<WeeklyPlan>, IWeeklyPlanRepository
	{
		public WeeklyPlanRepository(WeMealKitContext context) : base(context)
		{

		}



		public override Task<List<WeeklyPlan>> GetAllAsync()
		{
			return _dbSet
				.Include(wp => wp.RecipePLans)
					.ThenInclude(rp => rp.Recipe)
						.ThenInclude(r => r.RecipeCategories)
					.ThenInclude(rc => rc.Category) // Include Category through RecipeCategory
				.Include(wp => wp.RecipePLans)
					.ThenInclude(rp => rp.Recipe)
						.ThenInclude(r => r.RecipeIngredients)
							.ThenInclude(ri => ri.Ingredient)
								.ThenInclude(i => i.IngredientNutrient)
				.Include(wp => wp.RecipePLans)
					.ThenInclude(rp => rp.Recipe)
						.ThenInclude(r => r.RecipeIngredients)
							.ThenInclude(ri => ri.Ingredient)
								.ThenInclude(i => i.IngredientCategory)
				.Include(wp => wp.RecipePLans)
					.ThenInclude(rp => rp.Recipe)
						.ThenInclude(r => r.RecipeNutrient)
				.Include(wp => wp.RecipePLans)
					.ThenInclude(rp => rp.Recipe)
						.ThenInclude(r => r.RecipeSteps)
				.ToListAsync();
		}
		public override async Task<WeeklyPlan?> GetByIdAsync(string id)
		{
			try
			{
				Guid guidId;
				if ( !Guid.TryParse(id , out guidId) )
				{
					return null;
				}
				var entity = await _dbSet
					.Include(wp => wp.RecipePLans)
						.ThenInclude(rp => rp.Recipe)
							.ThenInclude(r => r.RecipeIngredients)//lay toi RecipeIngredient
								.ThenInclude(ri => ri.Ingredient)
									.ThenInclude(i => i.IngredientNutrient)//lay toi ingredientNutrient
					.Include(wp => wp.RecipePLans)
						.ThenInclude(rp => rp.Recipe)
							.ThenInclude(r => r.RecipeIngredients)
								.ThenInclude(ri => ri.Ingredient)
									.ThenInclude(i => i.IngredientCategory)//lay toi ingredientCategory
					  .Include(wp => wp.RecipePLans)
							.ThenInclude(rp => rp.Recipe)
								.ThenInclude(r => r.RecipeCategories)
					.Include(wp => wp.RecipePLans)
						.ThenInclude(rp => rp.Recipe)
							.ThenInclude(r => r.RecipeCategories)
								.ThenInclude(rc => rc.Category)
				   .Include(wp => wp.RecipePLans)
						.ThenInclude(rp => rp.Recipe)
							.ThenInclude(r => r.RecipeNutrient)//lay toi recipeNutrient
				   .Include(wp => wp.RecipePLans)
						.ThenInclude(rp => rp.Recipe)
							.ThenInclude(r => r.RecipeSteps)//lay toi recipeStep
					.FirstOrDefaultAsync(wp => wp.Id == guidId);
				return entity;
			}
			catch ( Exception ex )
			{
				Console.WriteLine($"Error occurred in GetAsync: {ex}");
				return null;
			}
		}

		public override IQueryable<WeeklyPlan> Get(Expression<Func<WeeklyPlan , bool>> expression)
		{
			return _dbSet
				.Where(expression)
				.Include(wp => wp.RecipePLans)
								.ThenInclude(rp => rp.Recipe)
									.ThenInclude(r => r.RecipeIngredients)//lay toi RecipeIngredient
										.ThenInclude(ri => ri.Ingredient)
											.ThenInclude(i => i.IngredientNutrient)//lay toi ingredientNutrient
						  .Include(wp => wp.RecipePLans)
									.ThenInclude(rp => rp.Recipe)
										.ThenInclude(r => r.RecipeIngredients)
											.ThenInclude(ri => ri.Ingredient)
												.ThenInclude(i => i.IngredientCategory)//lay toi ingredientCategory
						  .Include(wp => wp.RecipePLans)
								.ThenInclude(rp => rp.Recipe)
									.ThenInclude(r => r.RecipeCategories)//lay danh sach recipeCategory
										.ThenInclude(rc => rc.Category)
						   .Include(wp => wp.RecipePLans)
								.ThenInclude(rp => rp.Recipe)
									.ThenInclude(r => r.RecipeNutrient)//lay toi recipeNutrient
						   .Include(wp => wp.RecipePLans)
								.ThenInclude(rp => rp.Recipe)
									.ThenInclude(r => r.RecipeSteps);//lay toi recipeStep
		}

		public async Task<bool> RecipeExistInWeeklyPlanAsync(Guid weeklyPlanId)
		{
			var weeklyplan = await _dbSet.Include(w => w.RecipePLans).FirstOrDefaultAsync(w => w.Id == weeklyPlanId);
			if ( weeklyplan != null && weeklyplan.RecipePLans.Any() )
			{
				return true;
			}
			return false;
		}

		public async Task<List<WeeklyPlan>> GetAllWeeklyPlanFilterAsync(DateTime startOfWeek , DateTime enOfWeek)
		{
			return await _dbSet
				.Where(wp => wp.BeginDate >= startOfWeek && wp.EndDate <= enOfWeek)
				.Include(wp => wp.RecipePLans)
					.ThenInclude(rp => rp.Recipe)
						.ThenInclude(r => r.RecipeCategories)
							.ThenInclude(rc => rc.Category)
				.Include(wp => wp.RecipePLans)
					.ThenInclude(rp => rp.Recipe)
						.ThenInclude(r => r.RecipeIngredients)
							.ThenInclude(ri => ri.Ingredient)
				.ToListAsync();
		}
	}
}
