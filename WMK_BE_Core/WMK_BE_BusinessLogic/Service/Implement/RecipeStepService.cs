﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeStepModel.RecipeStep;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeStepModel.RecipeStep;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class RecipeStepService : IRecipeStepService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RecipeStepService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Get all
        public async Task<ResponseObject<RecipeStepRespone>> GetRecipeSteps()
        {
            var result = new ResponseObject<RecipeStepRespone>();
            var recipeStepList = await _unitOfWork.RecipeStepRepository.GetAllAsync();
            if (recipeStepList != null && recipeStepList.Count > 0)
            {
                result.StatusCode = 200;
                result.Message = "List all recipe steps";
                result.List = _mapper.Map<List<RecipeStepRespone>>(recipeStepList);
                return result;
            }
            else
            {
                result.StatusCode = 404;
                result.Message = "Recipe steps not found";
                return result;
            }
        }
        #endregion

        #region Get-by-recipe-id
        public async Task<ResponseObject<List<RecipeStepRespone>>> GetByRecipeId(Guid recipeId)
        {
            var result = new ResponseObject<List<RecipeStepRespone>>();
            var currentList = await _unitOfWork.RecipeStepRepository.GetAllAsync();
            if (currentList.Count == 0)
            {
                result.StatusCode = 400;
                result.Message = "Empty in db. Say from GetByRecipeId";
                return result;
            }
            List<RecipeStep> foundList = new List<RecipeStep>();
            foreach (var recipeStep in currentList)
            {
                if (recipeStep.RecipeId == recipeId)
                {
                    foundList.Add(recipeStep);
                }
            }
            if (foundList.Count == 0)
            {
                result.StatusCode = 400;
                result.Message = "Empty when find. Say from GetByRecipeId";
                return result;
            }
            result.StatusCode = 200;
            result.Message = "Recipe steps get from recipe id";
            result.Data = _mapper.Map<List<RecipeStepRespone>>(foundList);
            return result;
        }
        #endregion Get-by-recipe-id

        #region Create-multiple-steps-for-new-recipe
        public async Task<ResponseObject<List<RecipeStep>>> CreateRecipeSteps(Guid recipeId, List<CreateRecipeStepRequest> stepList)
        {
            var result = new ResponseObject<List<RecipeStep>>();
            /*
             xac nhan recipe
            chay vong lap tao recipe step
            -check trung lap
            -check index
             */
            var foundRecipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
            if(foundRecipe == null || foundRecipe.Id == null)
            {
                result.StatusCode=400;
                result.Message = "Not found recipe. Say from CreateRecipeSteps";
                return result;
            }
            RecipeStep newStep = new RecipeStep();
            List<RecipeStep> returnList = new List<RecipeStep>();
            newStep.RecipeId = recipeId;
            foreach (var step in stepList)
            {
                _mapper.Map<RecipeStep>(step);
                var createResult = await _unitOfWork.RecipeStepRepository.CreateAsync(newStep);
                if (!createResult)//cho nay tra ve luon thong tin cu the type nao bi tao loi
                {
                    result.StatusCode = 500;
                    result.Message = "Create new recipe step unsuccessfully!. Say from CreateRecipeStep-for loop";
                    result.Data = null;
                    return result;
                }
                await _unitOfWork.CompleteAsync();
                returnList.Add(newStep);
            }
            result.StatusCode = 200;
            result.Message = "OK. Say from CreateRecipeSteps";
            result.Data = returnList;
            return result;
        }
        #endregion

        //public async Task<ResponseObject<bool>> CreateSingleStep(Guid recipeId, CreateRecipeStepRequest recipeStep)
        //{
        //    var result
        //}
    }
}
