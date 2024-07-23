﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeIngredientDetailModel;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class RecipeIngredientOrderDetailService : IRecipeIngredientOrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RecipeIngredientOrderDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseObject<RecipeIngredientInOrderDetailResponse>> CreateRecipeIngredientOrderDetail()
        {

        }
    }
}
