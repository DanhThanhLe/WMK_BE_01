using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientNutrientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientNutrientModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Service.Interface
{
    public interface IIngredientNutrientService
    {
        Task<ResponseObject<List<IngredientNutrientResponse>>> GetAll();
        Task<ResponseObject<IngredientNutrientResponse>> GetById(Guid request);
        Task<ResponseObject<IngredientNutrientResponse>> GetByIngredientId(Guid request);
        Task<ResponseObject<IngredientNutrient>> Create(Guid ingredientId, CreateIngredientNutrientRequest request);
        Task<ResponseObject<IngredientNutrientResponse>> Update(Guid id, FullIngredientNutrientRequest request);
        Task<ResponseObject<IngredientNutrientResponse>> DeleteById(Guid request);
        


    }
}
