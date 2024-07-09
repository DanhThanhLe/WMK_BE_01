using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
	public class UnitOfWork : IUnitOfWork
	{
		public IIngredientRepository IngredientRepository { get; private set;}
		public IRecipeIngredientRepository RecipeIngredientRepository { get; private set;}
		public ICategoryRepository CategoryRepository { get; private set;}
		public IWeeklyPlanRepository WeeklyPlanRepository { get; private set;}
		public IRecipePlanRepository RecipePlanRepository { get; private set;}
		public IRecipeRepository RecipeRepository { get; private set;}
        public IRecipeStepRepository RecipeStepRepository { get; private set;}
        public IUserRepository UserRepository { get; private set;}
        public IOrderRepository OrderRepository { get; private set;}
        public ICustomPlanRepository CustomPlanRepository { get; private set;}
        public IOrderGroupRepository OrderGroupRepository { get; private set;}
        public IRecipeNutrientRepository RecipeNutrientRepository { get; private set;}
        public IIngredientCategoryRepository IngredientCategoryRepository { get; private set;}

        public IRecipeCategoryRepository RecipeCategoryRepository { get; private set; }
        public IIngredientNutrientRepository IngredientNutrientRepository { get; private set;}

        private readonly WeMealKitContext _context;
        public UnitOfWork(WeMealKitContext context)
        {
            this._context = context;
            IngredientRepository = new IngredientRepository(context);
			RecipeIngredientRepository = new RecipeIngredientRepository(context);
            CategoryRepository = new CategoryRepository(context);
            WeeklyPlanRepository = new WeeklyPlanRepository(context);
            RecipePlanRepository = new RecipePlanRepository(context);
            RecipeRepository = new RecipeRepository(context);
            RecipeCategoryRepository = new RecipeCategoryRepository(context);
            RecipeStepRepository = new RecipeStepRepository(context);
			UserRepository = new UserRepository(context);
            OrderRepository = new OrderRepository(context);
            CustomPlanRepository = new CustomPlanRepository(context);
            OrderGroupRepository = new OrderGroupRepository(context);
            RecipeNutrientRepository = new RecipeNutrientRepository(context);
            IngredientCategoryRepository = new IngredientCategoryRepository(context);
            IngredientNutrientRepository = new IngredientNutrientRepository(context) ;
		}
        public async Task CompleteAsync()
        {
            await this._context.SaveChangesAsync();
        }
    }
}
