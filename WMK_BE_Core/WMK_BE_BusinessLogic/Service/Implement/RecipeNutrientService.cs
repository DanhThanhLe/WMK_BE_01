﻿using AutoMapper;
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
        public RecipeNutrientService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseObject<RecipeNutrient>> Create(Guid recipeId, List<CreateRecipeIngredientRequest> recipeIngredients)
        {
            var result = new ResponseObject<RecipeNutrient>();//result
            var checkRecipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());//tim recipe
            if (checkRecipe == null)//khong tim thay recipe
            {
                result.StatusCode = 500;
                result.Message = "Not found Recipe id " + recipeId + ". Say from Create - RecipeNutrientService";
                return result;
            }
            else
            {
                //lay thong tin recipe nutrient coi co chua
                var currentList = await _unitOfWork.RecipeNutrientRepository.GetAllAsync();
                var foundRecipeNutrient = currentList.FirstOrDefault(x => x.RecipeID.ToString().Equals(recipeId.ToString()));
                if (foundRecipeNutrient != null)//co thong tin recipe nutrient ton tai roi -> bao loi ko cho tao moi nua
                {
                    result.StatusCode = 400;
                    result.Message = "Nutrient info with recipe id " + recipeId + " already existed, can not create more . Say from Create - RecipeNutrientService";
                    return result;
                }
                else
                {
                    CreateRecipeNutrientRequest createRequest = new CreateRecipeNutrientRequest();
                    createRequest.RecipeID = recipeId;

                    List<IngredientNutrient> listIngredientNutrient = await _unitOfWork.IngredientNutrientRepository.GetAllAsync();
                    if (listIngredientNutrient.Count() > 0)
                    {
                        foreach (var item in recipeIngredients)
                        {
                            IngredientNutrient foundIngredientNutrient = listIngredientNutrient.FirstOrDefault(x => x.IngredientID.ToString().Equals(item.IngredientId.ToString()));
                            if (foundIngredientNutrient.Id.ToString() != null)
                            {
                                createRequest.Calories += (foundIngredientNutrient.Calories * item.amount);
                                createRequest.Fat += (foundIngredientNutrient.Fat * item.amount);
                                createRequest.SaturatedFat += (foundIngredientNutrient.SaturatedFat * item.amount);
                                createRequest.Sugar += (foundIngredientNutrient.Sugar * item.amount);
                                createRequest.Carbonhydrate += (foundIngredientNutrient.Carbonhydrate * item.amount);
                                createRequest.DietaryFiber += (foundIngredientNutrient.DietaryFiber * item.amount);
                                createRequest.Protein += (foundIngredientNutrient.Protein * item.amount);
                                createRequest.Sodium += (foundIngredientNutrient.Sodium * item.amount);
                            }
                            else
                            {
                                result.StatusCode = 500;
                                result.Message = "not found nutrient for ingredient";
                                return result;
                            }

                        }
                        RecipeNutrient newOne = _mapper.Map<RecipeNutrient>(createRequest);
                        var createResult = await _unitOfWork.RecipeNutrientRepository.CreateAsync(newOne);
                        if (createResult)
                        {
                            await _unitOfWork.CompleteAsync();
                            result.StatusCode = 200;
                            result.Message = "OK create nutrient ok";
                            result.Data = newOne;
                            return result;
                        }
                        else
                        {
                            result.StatusCode = 400;
                            result.Message = "Error in create nutrient. Say from Create - RecipeNutrientService";
                            return result;
                        }
                    }
                    else
                    {
                        result.StatusCode = 400;
                        result.Message = "Not have ingredint nutrient. Say from Create - RecipeNutrientService";
                        return result;
                    }
                }
            }
        }


    }
}
