using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeStepModel.RecipeStep;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeStepModel.RecipeStep;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
    public class RecipeStepProfile : Profile
    {
        public RecipeStepProfile() 
        {
            CreateMap<RecipeStep, RecipeStepRespone>().ReverseMap();
            CreateMap<RecipeStep, CreateRecipeStepRequest>().ReverseMap();
        }
    }
}
