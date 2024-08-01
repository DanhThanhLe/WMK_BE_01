using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
	public class IngredientProfile : Profile
	{
		public IngredientProfile()
		{

			//model <-> request
			CreateMap<Ingredient , CreateIngredientRequest>().ReverseMap();
			//CreateMap<Ingredient , IngredientRequest>().ReverseMap();

			//model <-> response
			CreateMap<Ingredient , IngredientResponse>()
				//.ForMember(dest => dest.CategoryName , otp => otp.MapFrom(x => x.IngredientCategory.Name))
				//.ForMember(dest => dest.CategoryDescription , otp => otp.MapFrom(x => x.IngredientCategory.Description))
				.ReverseMap();
		}
	}
}
