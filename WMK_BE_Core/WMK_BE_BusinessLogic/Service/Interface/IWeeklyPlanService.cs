using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.WeeklyPlan;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface IWeeklyPlanService
	{
		Task<ResponseObject<WeeklyPlanResponseModel>> GetAllAsync();
		Task<ResponseObject<WeeklyPlanResponseModel?>> GetByIdAsync(Guid id);
	}
}
