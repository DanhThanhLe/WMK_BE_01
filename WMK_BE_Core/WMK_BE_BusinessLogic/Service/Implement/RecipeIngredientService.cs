﻿using AutoMapper;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeAmountModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeCategoryModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class RecipeIngredientService : IRecipeIngredientService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public RecipeIngredientService(IUnitOfWork unitOfWork , IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		#region Get
		public async Task<ResponseObject<List<RecipeIngredientResponse>>> GetAll()
		{
			var result = new ResponseObject<List<RecipeIngredientResponse>>();
			var list = await _unitOfWork.RecipeIngredientRepository.GetAllAsync();
			if ( list == null )
			{
				result.StatusCode = 400;
				result.Message = "Không tìm thấy bản ghi";
				return result;
			}
			result.StatusCode = 200;
			result.Message = "Tìm thấy";
			result.Data = _mapper.Map<List<RecipeIngredientResponse>>(list);
			return result;
		}
		#endregion

		#region Get-by-recipe-id
		public async Task<ResponseObject<List<RecipeIngredientResponse>>> GetListByRecipeId(Guid recipeId)
		{
			var result = new ResponseObject<List<RecipeIngredientResponse>>();
			var checkRecipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
			if ( checkRecipe == null )
			{
				result.StatusCode = 400;
				result.Message = "Công thức không tồn tại";
				return result;
			}
			var currentList = await _unitOfWork.RecipeIngredientRepository.GetAllAsync();
			var foundList = new List<RecipeIngredient>();
			foreach ( var item in currentList )
			{
				if ( item.RecipeId.Equals(recipeId) )
				{
					foundList.Add(item);
				}
			}
			if ( foundList.Count == 0 )
			{
				result.StatusCode = 500;
				result.Message = "Không tìm thấy bản ghi" + recipeId;
				return result;
			}
			result.StatusCode = 200;
			result.Message = "Tìm thấy";
			result.Data = _mapper.Map<List<RecipeIngredientResponse>>(foundList);
			return result;
		}
		#endregion

		#region create recipe ingredient with recipe create
		public async Task<ResponseObject<List<RecipeIngredient>?>> CreateRecipeIngredientAsync(Guid recipeId , List<CreateRecipeIngredientRequest> recipeIngredientRequests) //chua kiem tra trung id ingredient
		{
			var result = new ResponseObject<List<RecipeIngredient>?>();
			var recipeIngredients = new List<RecipeIngredient>();
			try
			{
				//check recipe exist
				var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());

				if ( recipeExist == null )
				{
					result.StatusCode = 404;
					result.Message = "Không tìm thấy bản ghi";
					return result;
				}
				//scan list ingredient
				foreach ( var ingredient in recipeIngredientRequests )
				{
					var ingredientEixst = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredient.IngredientId.ToString());
					if ( ingredientEixst == null )
					{

						result.StatusCode = 404;
						result.Message = "Không tìm thấy bản ghi";
						return result;
					}
					RecipeIngredient newRecipeIngredient = new RecipeIngredient();
					newRecipeIngredient.RecipeId = recipeId;
					newRecipeIngredient.IngredientId = ingredient.IngredientId;
					newRecipeIngredient.Amount = ingredient.amount;
					newRecipeIngredient.Recipe = recipeExist;
					newRecipeIngredient.Ingredient = ingredientEixst;
					recipeIngredients.Add(newRecipeIngredient);
				}
				if ( recipeIngredients.Any() )
				{
					await _unitOfWork.RecipeIngredientRepository.AddRangeAsync(recipeIngredients);
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Tạo thành công.";
					result.Data = recipeIngredients;
					return result;
				}
				result.StatusCode = 500;
				result.Message = "Tạo không thành công";
				return result;
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
				return result;
			}
		}


		#endregion

		#region Delete
		public async Task<ResponseObject<List<RecipeIngredient>?>> DeleteRecipeIngredientByRecipeAsync(Guid recipeId)
		{
			var result = new ResponseObject<List<RecipeIngredient>?>();
			try
			{
				//check recipe exist
				var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
				if ( recipeExist == null )
				{
					result.StatusCode = 404;
					result.Message = "Không tìm thấy bản ghi";
					return result;
				}
				//take recipeIngredients asign to recipe
				var recipeIngredients = _unitOfWork.RecipeIngredientRepository.GetAll()
					.Where(rc => rc.RecipeId == recipeId);
				if ( recipeIngredients == null || !recipeIngredients.Any() )
				{
					result.StatusCode = 200;
					result.Message = "Không tìm thấy thông tin";
					return result;
				}
				// Xóa các recipeIngredients liên quan
				_unitOfWork.RecipeIngredientRepository.DeleteRange(recipeIngredients);
				await _unitOfWork.CompleteAsync();

				result.StatusCode = 200;
				result.Message = "Xóa thành công";
				result.Data = recipeIngredients.ToList();
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
			}
			return result;
		}
		#endregion

	}
}
