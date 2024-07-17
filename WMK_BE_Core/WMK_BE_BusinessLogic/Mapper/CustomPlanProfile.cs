using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.CustomPlanModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
    public class CustomPlanProfile : Profile
    {
        public CustomPlanProfile()
        {
            CreateMap<CustomPlan, CreateCustomPlanRequest>().ReverseMap();
            CreateMap<CustomPlan, CustomPlanResponse>().ReverseMap();
        }
    }
}
