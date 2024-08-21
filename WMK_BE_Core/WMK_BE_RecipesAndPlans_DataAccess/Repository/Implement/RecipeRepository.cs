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



		public async Task<bool> ChangeStatusAsync(Guid id , ProcessStatus status)
		{
			var recipe = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
			if ( recipe != null )
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
								//.AsNoTracking() // Sử dụng AsNoTracking để tăng hiệu suất khi không cần theo dõi
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
								//.Select(r => new Recipe // Chỉ chọn các trường cần thiết
								//{
								//	Id = r.Id ,
								//	Name = r.Name ,
								//	// Các trường khác của Recipe nếu cần
								//	RecipeIngredients = r.RecipeIngredients.Select(ri => new RecipeIngredient
								//	{
								//		Id = ri.Id ,
								//		Ingredient = new Ingredient
								//		{
								//			Id = ri.Ingredient.Id ,
								//			Name = ri.Ingredient.Name ,
								//			// Các trường khác của Ingredient nếu cần
								//			IngredientNutrient = ri.Ingredient.IngredientNutrient.Select(n => new IngredientNutrient
								//			{
								//				Id = n.Id ,
								//				NutrientName = n.NutrientName ,
								//				// Các trường khác của IngredientNutrient nếu cần
								//			}).ToList() ,
								//			IngredientCategory = ri.Ingredient.IngredientCategory
								//		}
								//	}).ToList() ,
								//	RecipeCategories = r.RecipeCategories.Select(rc => new RecipeCategory
								//	{
								//		Id = rc.Id ,
								//		Category = new Category
								//		{
								//			Id = rc.Category.Id ,
								//			Name = rc.Category.Name ,
								//			// Các trường khác của Category nếu cần
								//		}
								//	}).ToList() ,
								//	RecipeNutrient = r.RecipeNutrient.Select(rn => new RecipeNutrient
								//	{
								//		Id = rn.Id ,
								//		NutrientName = rn.NutrientName ,
								//		// Các trường khác của RecipeNutrient nếu cần
								//	}).ToList() ,
								//	RecipeSteps = r.RecipeSteps.Select(rs => new RecipeStep
								//	{
								//		Id = rs.Id ,
								//		StepDescription = rs.StepDescription ,
								//		// Các trường khác của RecipeStep nếu cần
								//	}).ToList()
								//})
								.ToListAsync();

			return recipes;
		}


		public override async Task<Recipe> GetByIdAsync(string id)
		{
			try
			{
				Guid guidId;
				if ( !Guid.TryParse(id , out guidId) )
				{
					return null;
				}

				var recipe = await _dbSet
								//.AsNoTracking()
								.Include(r => r.RecipeIngredients)
									.ThenInclude(ri => ri.Ingredient)
										.ThenInclude(i => i.IngredientNutrient)
								.Include(r => r.RecipeIngredients)
									.ThenInclude(ri => ri.Ingredient)
										.ThenInclude(i => i.IngredientCategory)
								.Include(r => r.RecipeCategories)
									.ThenInclude(rc => rc.Category)
								.Include(r => r.RecipePlans)
								.Include(r => r.RecipeNutrient)
								.Include(r => r.RecipeSteps)
								.FirstOrDefaultAsync(r => r.Id == guidId);

				return recipe;
			}
			catch ( Exception ex )
			{
				Console.WriteLine($"Error occurred in GetByIdAsync: {ex}");
				return null;
			}
		}




		public override IQueryable<Recipe> Get(Expression<Func<Recipe , bool>> expression)
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

		//public override async Task<bool> DeleteAsync(string id)
		//{
		//	try
		//	{
		//		// Tìm kiếm recipe cùng với các thực thể liên quan
		//		var recipe = await _dbSet.FindAsync(id);

		//		if ( recipe != null )
		//		{
		//			// Xóa recipe
		//			_dbSet.Remove(recipe);
		//			return true;
		//		}

		//		return false; // Không tìm thấy recipe với id đã cho
		//	}
		//	catch ( Exception ex )
		//	{
		//		// Xử lý ngoại lệ và ghi log nếu cần
		//		// Có thể sử dụng một công cụ logging như Serilog hoặc NLog để ghi log chi tiết ngoại lệ
		//		throw new Exception("Error deleting recipe" , ex);
		//	}
		//}

	}
}
