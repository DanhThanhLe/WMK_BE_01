﻿using AutoMapper;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class RecipeAmountService : IRecipeAmountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RecipeAmountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ResponseObject<List<RecipeAmount>?>> CreateRecipeAmountAsync(Guid recipeId, List<RecipeAmountCreateModel> ingredientModels) //chua kiem tra trung id ingredient
        {
            var result = new ResponseObject<List<RecipeIngredient>?>();
            var recipeAmounts = new List<RecipeIngredient>();
            try
            {
                //check recipe exist
                var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());

                if (recipeExist == null)
                {
                    result.StatusCode = 404;
                    result.Message = "Recipe not exist!";
                    return result;
                }
                //scan list ingredient
                foreach (var ingredient in ingredientModels)
                {
                    var ingredientEixst = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredient.IngredientId.ToString());
                    if (ingredientEixst != null)
                    {
                        var recipeAmount = new RecipeIngredient
                        {
                            Ingredient = ingredientEixst,
                            IngredientId = ingredient.IngredientId,
                            RecipeId = recipeId,
                            Recipe = recipeExist,
                            Amount = ingredient.amount,
                        };
                        recipeAmounts.Add(recipeAmount);
                    }
                    else
                    {
                        result.StatusCode = 404;
                        result.Message = $"Ingredient with ID {ingredient.IngredientId} not found!";
                        return result;
                    }
                }
                if (recipeAmounts.Any())
                {
                    await _unitOfWork.RecipeAmountRepository.AddRangeAsync(recipeAmounts);
                }
                await _unitOfWork.CompleteAsync();
                result.StatusCode = 200;
                result.Message = "Create recipe amount successfully.";
                result.Data = recipeAmounts;
                return result;
            }
            catch (Exception ex)
            {
                result.StatusCode = 500;
                result.Message = ex.Message;
                return result;
            }
        }
        public async Task<ResponseObject<RecipeAmountResponse>> GetAll()
        {
            var result = new ResponseObject<RecipeAmountResponse>();
            var list = await _unitOfWork.RecipeAmountRepository.GetAllAsync();
            if (list == null && list.Count == 0)
            {
                result.StatusCode = 400;
                result.Message = "Not found. Empty list recipe amount";
                return result;
            }
            result.StatusCode = 200;
            result.Message = "OK. list recipe amount";
            result.List = _mapper.Map<List<RecipeAmountResponse>>(list);
            return result;
        }

        #region Get-by-recipe-id dang sua
        public async Task<ResponseObject<RecipeAmountResponse>> GetListByRecipeId(Guid recipeId)
        {
            var result = new ResponseObject<RecipeAmountResponse>();
            var checkRecipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
            if (checkRecipe == null)
            {
                result.StatusCode = 400;
                result.Message = "Recipe not existed";
                return result;
            }
            var currentList = await _unitOfWork.RecipeAmountRepository.GetAllAsync();
            var foundList = new List<RecipeIngredient>();
            foreach (var item in currentList)
            {
                if (item.RecipeId.Equals(recipeId))
                {
                    foundList.Add(item);
                }
            }
            if (foundList.Count == 0)
            {
                result.StatusCode = 500;
                result.Message = "Not found List for Id: " + recipeId;
                return result;
            }
            result.StatusCode = 200;
            result.Message = "Ok. Recipe category list:";
            result.List = _mapper.Map<List<RecipeAmountResponse>>(foundList);
            return result;
        }
        #endregion
    }
}
