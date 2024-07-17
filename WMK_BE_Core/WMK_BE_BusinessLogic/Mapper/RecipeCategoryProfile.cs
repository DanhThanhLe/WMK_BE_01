using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeCategoryModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
    public class RecipeCategoryProfile : Profile
    {
        public RecipeCategoryProfile()
        {
            CreateMap<RecipeCategory, RecipeCategoryResponse>()
            //.ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category.Id))
            //.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Category.Type))//Mapping type của Category
            //.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Category.Name)) // Mapping tên Category
                .ReverseMap();

            CreateMap<RecipeCategory, CreateRecipeCategoryRequest>().ReverseMap();
        }
    }
}
