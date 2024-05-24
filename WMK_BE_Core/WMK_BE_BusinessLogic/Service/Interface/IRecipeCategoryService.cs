using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeCategoryModel;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
    public interface IRecipeCategoryService
    {
        Task<ResponseObject<List<RecipeCategoryResponse>?>> Create(Guid recipeId, List<Guid> categoryList);
        Task<ResponseObject<RecipeCategoryResponse>> Update(RecipeCategoryRequest recipeCategory);
        //Task<ResponseObject<RecipeCategoryResponse>> GetByRecipeId(Guid recipeId);
    }
}
