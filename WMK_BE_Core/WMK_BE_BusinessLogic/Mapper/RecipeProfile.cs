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
			CreateMap<Recipe , CreateRecipeRequest>()
			.ForMember(dest => dest.RecipeIngredientsList, opt => opt.MapFrom(src => src.RecipeIngredients))
			.ReverseMap();
			// Ánh xạ từ CreateRecipeIngredientRequest sang RecipeIngredient
			CreateMap<CreateRecipeIngredientRequest , RecipeIngredient>()
				.ForMember(dest => dest.IngredientId , opt => opt.MapFrom(src => src.IngredientId))
				.ForMember(dest => dest.Amount , opt => opt.MapFrom(src => src.amount))
				.ReverseMap();
			CreateMap<Recipe , UpdateRecipeRequest>().ReverseMap();
			CreateMap<Recipe , ChangeRecipeStatusRequest>().ReverseMap();
			CreateMap<Recipe , RecipeResponse>().ReverseMap();
			CreateMap<Recipe , ChangeRecipeBaseStatusRequest>().ReverseMap();


			//CreateMap//chua co map cho request voi model nen ko tao đc fuck
		}
	}
}