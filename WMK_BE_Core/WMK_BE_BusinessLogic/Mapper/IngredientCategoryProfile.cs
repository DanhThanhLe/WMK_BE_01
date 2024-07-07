using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientCategoryModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
    public class IngredientCategoryProfile : Profile
    {
        public IngredientCategoryProfile()
        {
            CreateMap<IngredientCategory,CreateIngredientCategoryRequest>().ReverseMap();
            CreateMap<IngredientCategory, FullIngredientCategoryRequest>().ReverseMap();

            CreateMap<IngredientCategory,IngredientCategoryResponse>().ReverseMap();
        }
    }
}
