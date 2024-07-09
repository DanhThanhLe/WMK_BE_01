using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientNutrientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientNutrientModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
    public class IngredientNutrientProfile : Profile
    {
        public IngredientNutrientProfile()
        {
            CreateMap<IngredientNutrient,CreateIngredientNutrientRequest>().ReverseMap();
            CreateMap<IngredientNutrient, FullIngredientNutrientRequest>().ReverseMap();

            CreateMap<IngredientNutrient,IngredientNutrientResponse>().ReverseMap();
        }
    }
}
