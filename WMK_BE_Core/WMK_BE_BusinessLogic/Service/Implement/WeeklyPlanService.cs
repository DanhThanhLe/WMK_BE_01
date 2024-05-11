using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.WeeklyPlan;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class WeeklyPlanService : IWeeklyPlanService
	{
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WeeklyPlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
             _unitOfWork = unitOfWork;
			_mapper = mapper;
        }

		public async Task<ResponseObject<WeeklyPlanResponseModel>> GetAllAsync()
		{
			var result = new ResponseObject<WeeklyPlanResponseModel>();
			var weeklyPlans = await _unitOfWork.WeeklyPlanRepository.GetAllAsync();
			if(weeklyPlans != null )
			{
				result.StatusCode = 200;
				result.Message = "WeeklyPlan list: ";
				result.List = _mapper.Map<List<WeeklyPlanResponseModel>>(weeklyPlans);
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Not have plan!";
				return result;
			}
		}

		public Task<ResponseObject<WeeklyPlanResponseModel?>> GetByIdAsync(Guid id)
		{
			throw new NotImplementedException();
		}
	}
}
