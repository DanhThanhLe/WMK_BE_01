using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.NutritionModel;

namespace WMK_BE_BusinessLogic.Mapper
{
    public class NutritionProfile:Profile
    {
        public NutritionProfile()
        {
            CreateMap<NutritionProfile,CreateNutritionRequest>().ReverseMap();
        }
    }
}
