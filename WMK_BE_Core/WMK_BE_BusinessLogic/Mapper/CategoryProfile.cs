using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.CategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.CategoryModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
	public class CategoryProfile : Profile
	{
        public CategoryProfile()
        {
            CreateMap<Category, CategoryResponseModel>().ReverseMap();
                //.ForMember(dest => dest.Status, otp => otp.MapFrom(src => src.Status.ToString()));
            CreateMap<Category, CreateCategoryRequestModel>().ReverseMap();
            CreateMap<Category, CategoryResponseInRecipeCategory>().ReverseMap();
        }
    }
}
