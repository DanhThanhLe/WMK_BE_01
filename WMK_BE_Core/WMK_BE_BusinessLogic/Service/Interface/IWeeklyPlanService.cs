using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.WeeklyPlanModel;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface IWeeklyPlanService
	{
		Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetAllAsync();
		Task<ResponseObject<WeeklyPlanResponseModel?>> GetByIdAsync(Guid id);
		Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetListByCustomerId(Guid customerId);

        Task<ResponseObject<WeeklyPlanResponseModel>> CreateWeeklyPlanAsync(CreateWeeklyPlanRequest model);
		Task<ResponseObject<WeeklyPlanResponseModel>> CreateForSutomer(CreateWeeklyPlanRequest request);

        Task<ResponseObject<WeeklyPlanResponseModel>> UpdateWeeklyPlanAsync(UpdateWeeklyPlanRequestModel model);
		Task<ResponseObject<WeeklyPlanResponseModel>> DeleteWeeklyPlanAsync(DeleteWeeklyPlanRequestModel model);
		Task<ResponseObject<WeeklyPlanResponseModel>> ChangeStatusWeeklyPlanAsync(ChangeStatusWeeklyPlanRequestModel model);
	}
}

