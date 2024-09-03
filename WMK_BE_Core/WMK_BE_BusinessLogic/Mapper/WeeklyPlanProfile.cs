using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.WeeklyPlanModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
	public class WeeklyPlanProfile : Profile
	{
        public WeeklyPlanProfile()
        {
            CreateMap<WeeklyPlan, WeeklyPlanResponseModel>().ForMember(dest => dest.RecipePLans, opt => opt.MapFrom(src => src.RecipePLans)).ReverseMap();
            CreateMap<WeeklyPlan, CreateWeeklyPlanRequest>().ReverseMap();
            CreateMap<WeeklyPlan, UpdateWeeklyPlanRequestModel>().ReverseMap();
            CreateMap<WeeklyPlan, UpdateWeeklyPlanRequest>().ReverseMap();
            CreateMap<WeeklyPlan, CreateWeeklyPlanForCustomerRequest>().ReverseMap();
            CreateMap<WeeklyPlan, Chang>().ReverseMap();
        }
    }
}
