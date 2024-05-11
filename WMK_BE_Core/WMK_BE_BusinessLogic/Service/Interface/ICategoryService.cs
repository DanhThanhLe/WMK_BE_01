using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Category;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Category;
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
	}
}
