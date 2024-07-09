using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeAmountModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
    public class RecipeIngredientProfile : Profile
    {
        public RecipeIngredientProfile()
        {

            CreateMap<RecipeIngredient,RecipeIngredientResponse>().ReverseMap();

        }
    }
}
