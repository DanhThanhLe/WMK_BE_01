using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
    public class RecipeProfile : Profile
    {
        public RecipeProfile()
        {
            CreateMap<Recipe, RecipeResponse>().ReverseMap();
            CreateMap<Recipe, CreateRecipeRequest>()
                .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.RecipeCategories.Select(rc => rc.Id)))
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.RecipeAmounts))
                .ForMember(dest => dest.Nutrition, opt => opt.MapFrom(src => src.Nutrition))
                .ReverseMap();
            //CreateMap//chua co map cho request voi model nen ko tao đc fuck
        }
    }
}
