using AutoMapper;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.NutritionModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeNutrientModel;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class RecipeNutrientService : IRecipeNutrientService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public RecipeNutrientService(IUnitOfWork unitOfWork , IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ResponseObject<RecipeNutrient>> CreateRecipeNutrientAsync(Guid recipeId , List<CreateRecipeNutrientRequest> recipeNutrient)
		{
			var result = new ResponseObject<RecipeNutrient>();//result
			var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());//tim recipe
			if ( recipeExist != null )
			{
				if ( recipeExist.RecipeNutrient == null )
				{
					//create recipe nutrient
					var recipeNutrientModel = _mapper.Map<RecipeNutrient>(recipeNutrient);
					var createResult = await _unitOfWork.RecipeNutrientRepository.CreateAsync(recipeNutrientModel);
					if ( createResult )
					{
						//success
						await _unitOfWork.CompleteAsync();
						result.StatusCode = 200;
						result.Message = "Create recipe nutrient success!";
						return result;
					}
					result.StatusCode = 500;
					result.Message = "Create recipe nutrient unsuccess!";
					return result;
				}
				result.StatusCode = 500;
				result.Message = "Recipe nutrient exist!";
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Recipe not exist!";
			return result;

		}


	}
}
