using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.NutritionModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeNutrientModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
    public class RecipeNutrientProfile:Profile
    {
        public RecipeNutrientProfile()
        {
            CreateMap<RecipeNutrient,CreateRecipeNutrientRequest>().ReverseMap();
            CreateMap<RecipeNutrient,RecipeNutrientResponse>().ReverseMap();
        }
    }
}
