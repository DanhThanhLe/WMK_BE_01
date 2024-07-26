using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeIngredientOrderDetailModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
    public class RecipeIngredientOrderDetailProfile : Profile
    {
        public RecipeIngredientOrderDetailProfile()
        {
            CreateMap<RecipeIngredientOrderDetail, RecipeIngredientOrderDetailResponse>().ReverseMap();
        }
    }
}
