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
        Task<ResponseObject<List<RecipeCategory>?>> Create(Guid recipeId, List<Guid> categoryList);
        Task<ResponseObject<List<RecipeCategory>?>> DeleteByRcipe(Guid recipeId);
        Task<ResponseObject<RecipeCategoryResponse>> Update(RecipeCategoryRequest recipeCategory);
        //Task<ResponseObject<RecipeCategoryResponse>> GetByRecipeId(Guid recipeId);
        List<Guid> GetRecipeIdByCategoryId(Guid categoryId);
        Task<ResponseObject<List<RecipeCategoryResponse>>> GetAll(string name="");
        Task<ResponseObject<List<RecipeCategoryResponse>>> GetListByRecipeId(Guid recipeId);
    }
}
