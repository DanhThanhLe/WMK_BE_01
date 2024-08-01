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
		Task<ResponseObject<List<CategoryResponseModel>>> GetAllAsync();
		Task<ResponseObject<CategoryResponseModel?>> GetByIdAsync(Guid id);
		Task<ResponseObject<CategoryResponseModel>> CreateCategoryAsync(CreateCategoryRequestModel model);
		Task<ResponseObject<CategoryResponseModel>> UpdateCategoryAsync(Guid id, UpdateCategoryRequestModel model);
		Task<ResponseObject<CategoryResponseModel>> ChangeCategoryAsync(Guid id, ChangeCategoryRequest model);
		Task<ResponseObject<CategoryResponseModel>> DeleteCategoryAsync(Guid id);
		Task<ResponseObject<List<CategoryResponseModel>>> GetCategoryByType(string type);
		Task<ResponseObject<List<CategoryResponseModel>>> GetcategoryByName(string name);

    }
}
