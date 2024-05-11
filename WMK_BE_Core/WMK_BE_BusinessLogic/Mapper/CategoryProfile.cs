using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Category;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Category;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
	public class CategoryProfile : Profile
	{
        public CategoryProfile()
        {
            CreateMap<Category, CategoryResponseModel>();
            CreateMap<Category, CreateCategoryRequestModel>();
        }
    }
}
