using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeIngredientDetailModel;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
    public interface IRecipeIngredientOrderDetailService
    {
        Task<ResponseObject<RecipeIngredientInOrderDetailResponse>> CreateRecipeIngredientOrderDetail(Guid orderDetailId, Guid recipeId);
    }
}
