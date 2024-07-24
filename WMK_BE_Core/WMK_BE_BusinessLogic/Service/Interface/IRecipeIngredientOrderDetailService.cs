using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeIngredientDetailModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Service.Interface
{
    public interface IRecipeIngredientOrderDetailService
    {
        Task<ResponseObject<List<RecipeIngredientOrderDetail>>> CreateRecipeIngredientOrderDetail(Guid orderDetailId, Guid recipeId, int recipeQuantity);
    }
}
