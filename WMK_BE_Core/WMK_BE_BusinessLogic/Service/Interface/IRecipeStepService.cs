﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeStepModel.RecipeStep;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeStepModel.RecipeStep;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Service.Interface
{
    public interface IRecipeStepService
    {
        Task<ResponseObject<RecipeStepRespone>> GetRecipeSteps();
        Task<ResponseObject<List<RecipeStepRespone>>> GetByRecipeId(Guid recipeId);
        Task<ResponseObject<List<RecipeStep>>> CreateRecipeSteps(Guid recipeId, List<CreateRecipeStepRequest> stepList);
    }
}
