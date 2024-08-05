using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeCategoryModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
    public class RecipeProfile : Profile
    {
        public RecipeProfile()
        {
            CreateMap<Recipe, CreateRecipeRequest>().ReverseMap();
            CreateMap<Recipe, RecipeResponse>()
                //.ForMember(dest => dest.RecipeCategories, opt => opt.MapFrom(src => src.RecipeCategories.Select(rc => new RecipeCategoryResponse
                //{
                //    CategoryId = rc.Category.Id,
                //    Type = rc.Category.Type,
                //    Name = rc.Category.Name  // Mapping tên Category
                //}).ToList()))
                .ReverseMap();

            CreateMap<Recipe , UpdateRecipeRequest>().ReverseMap();
            CreateMap<Recipe , ChangeRecipeStatusRequest>().ReverseMap();
            CreateMap<Recipe , ChangeRecipeBaseStatusRequest>().ReverseMap();

                
            //CreateMap//chua co map cho request voi model nen ko tao đc fuck
        }
    }
}
                //.ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.RecipeCategories.Select(rc => rc.Id)))
                //.ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.RecipeAmounts))
                //.ForMember(dest => dest.Nutrition, opt => opt.MapFrom(src => src.Nutrition))