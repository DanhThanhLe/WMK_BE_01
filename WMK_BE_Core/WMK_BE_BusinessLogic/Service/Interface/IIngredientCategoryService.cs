using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientCategoryModel;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
    public interface IIngredientCategoryService
    {
		#region Get
		Task<ResponseObject<List<IngredientCategoryResponse>>> GetAllAsync(GetAllIngredientCategoriesRequest? model);
        Task<ResponseObject<IngredientCategoryResponse>> GetById(Guid request);
        Task<ResponseObject<List<IngredientCategoryResponse>>> GetByNameAsync(string request);
		#endregion

        Task<ResponseObject<IngredientCategoryResponse>> CreateNew(CreateIngredientCategoryRequest request);
        
        Task<ResponseObject<IngredientCategoryResponse>> UpdateCategory(Guid id, FullIngredientCategoryRequest request);
        
        Task<ResponseObject<IngredientCategoryResponse>> DeleteById(Guid id);
        
        Task<ResponseObject<IngredientCategoryResponse>> ChangeStatusIngredientCategoryAsync(Guid id, ChangeStatusIngredientCategoryRequest request);
    }
}
