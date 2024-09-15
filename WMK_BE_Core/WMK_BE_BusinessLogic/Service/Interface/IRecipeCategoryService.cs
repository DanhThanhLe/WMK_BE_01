using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeCategoryModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface IRecipeCategoryService
	{
		#region Get
		Task<ResponseObject<List<RecipeCategoryResponse>>> GetAll(string name = "");
		List<Guid> GetRecipeIdByCategoryId(Guid categoryId);
		Task<ResponseObject<List<RecipeCategoryResponse>>> GetListByRecipeId(Guid recipeId);
		#endregion

		Task<ResponseObject<List<RecipeCategory>?>> Create(Guid recipeId , List<Guid> categoryList);

		Task<ResponseObject<RecipeCategoryResponse>> Update(RecipeCategoryRequest recipeCategory);

		Task<ResponseObject<List<RecipeCategory>?>> DeleteByRcipe(Guid recipeId);
	}
}
