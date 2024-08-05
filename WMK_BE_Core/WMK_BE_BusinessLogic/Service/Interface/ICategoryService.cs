using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.CategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.CategoryModel;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface ICategoryService
	{
		Task<ResponseObject<List<CategoryResponseModel>>> GetAllAsync(GetAllCategoriesRequest? model);
		Task<ResponseObject<CategoryResponseModel?>> GetByIdAsync(Guid id);
		Task<ResponseObject<List<CategoryResponseModel>>> GetCategoriesByTypeAsync(string type);
		Task<ResponseObject<List<CategoryResponseModel>>> GetcategoriesByNameAsync(string name);
		Task<ResponseObject<CategoryResponseModel>> CreateCategoryAsync(CreateCategoryRequest model);
		Task<ResponseObject<CategoryResponseModel>> UpdateCategoryAsync(Guid id, UpdateCategoryRequest model);
		Task<ResponseObject<CategoryResponseModel>> ChangeCategoryStatusAsync(Guid id, ChangeCategoryRequest model);
		Task<ResponseObject<CategoryResponseModel>> DeleteCategoryAsync(Guid id);

    }
}
