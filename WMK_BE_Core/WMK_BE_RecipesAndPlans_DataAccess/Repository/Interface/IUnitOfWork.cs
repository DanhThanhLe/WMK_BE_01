using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface
{
	public interface IUnitOfWork
	{
		IIngredientRepository IngredientRepository { get; }
		IRecipeIngredientRepository RecipeIngredientRepository { get; }
		ICategoryRepository CategoryRepository { get; }
		IWeeklyPlanRepository WeeklyPlanRepository { get; }
		IRecipePlanRepository RecipePlanRepository { get; }
		IRecipeRepository RecipeRepository { get; }
		IRecipeCategoryRepository RecipeCategoryRepository { get; }
		IRecipeStepRepository RecipeStepRepository { get; }
		IUserRepository UserRepository { get; }
		IOrderRepository OrderRepository { get; }
		ICustomPlanRepository CustomPlanRepository { get; }
		IOrderGroupRepository OrderGroupRepository { get; }
		IRecipeNutrientRepository RecipeNutrientRepository { get; }
		IIngredientCategoryRepository IngredientCategoryRepository { get; }
		IIngredientNutrientRepository IngredientNutrientRepository { get; }

		Task CompleteAsync();
	}
}
