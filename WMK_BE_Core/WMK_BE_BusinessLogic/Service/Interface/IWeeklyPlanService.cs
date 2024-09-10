using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.WeeklyPlanModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface IWeeklyPlanService
	{
		Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetAllAsync(string name = "");
		Task<ResponseObject<List<WeeklyPlanResponseModelForWeb>>> GetAllFilterAsync(GetAllRequest? model);
		Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetListByCustomerId(Guid customerId);
		Task<ResponseObject<WeeklyPlanResponseModel?>> GetByIdAsync(Guid id);
		Task<ResponseObject<WeeklyPlanResponseModel>> CreateWeeklyPlanAsync(CreateWeeklyPlanRequest model , string createdBy);
		Task<ResponseObject<WeeklyPlanResponseModel>> CreateForSutomer(CreateWeeklyPlanForCustomerRequest request);
		Task<ResponseObject<WeeklyPlanResponseModel>> UpdateWeeklyPlanAsync(Guid id , UpdateWeeklyPlanRequestModel model);
		Task<ResponseObject<WeeklyPlanResponseModel>> DeleteWeeklyPlanAsync(Guid id);
		Task<ResponseObject<WeeklyPlanResponseModel>> ChangeStatusWeeklyPlanAsync(string? userId , Guid id , ChangeStatusWeeklyPlanRequest model);
		Task<ResponseObject<WeeklyPlanResponseModel>> ChangeBaseStatusWeeklyPlanAsync(Guid id , ChangeBaseStatusWeeklyPlanRequest model);
		Task<ResponseObject<WeeklyPlanResponseModel>> UpdateFullInfo(Guid id , UpdateWeeklyPlanRequest request);
		Task<ResponseObject<WeeklyPlanResponseModel>> OnOffOrderAsync(bool status);

	}
}

