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
		Task<ResponseObject<CategoryResponseModel>> GetAllAsync();
		Task<ResponseObject<CategoryResponseModel?>> GetByIdAsync(Guid id);
		Task<ResponseObject<CategoryResponseModel>> CreateCategoryAsync(CreateCategoryRequestModel model);
		Task<ResponseObject<CategoryResponseModel>> UpdateCategoryAsync(UpdateCategoryRequestModel model);
		Task<ResponseObject<CategoryResponseModel>> DeleteCategoryAsync(DeleteCategoryRequestModel model);
		Task<ResponseObject<CategoryResponseModel>> GetCategoryByType(string type);
		Task<ResponseObject<CategoryResponseModel>> GetcategoryByName(string name);

    }
}
