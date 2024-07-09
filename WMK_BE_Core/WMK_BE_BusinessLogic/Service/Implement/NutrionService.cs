using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.NutritionModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class NutritionService : INutritionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public NutritionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseObject<Nutrition>> CreateNutritionInfo(Guid recipeId, CreateNutritionRequest nutrition)
        {
            var result = new ResponseObject<Nutrition>();
            var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
            if (recipeExist == null)
            {
                result.StatusCode = 404;
                result.Message = "Recipe not exist! Say from CreateNutritionInfo in NutritionService";
                return result;
            }
            Nutrition newInfo = new Nutrition
            {
                RecipeID = recipeId,
                Calories = nutrition.Calories,
                Fat = nutrition.Fat,
                SaturatedFat = nutrition.SaturatedFat,
                Sugar = nutrition.Sugar,
                Carbonhydrate = nutrition.Carbonhydrate,
                DietaryFiber = nutrition.DietaryFiber,
                Protein = nutrition.Protein,
                Sodium = nutrition.Sodium
            };
            var createResult = await _unitOfWork.NutritionRepository.CreateAsync(newInfo);
            if (!createResult)//cho nay tra ve luon thong tin cu the type nao bi tao loi
            {
                result.StatusCode = 500;
                result.Message = "Create new recipe's nutrition info unsuccessfully!. Say from CreateNutritionInfo in NutritionService";
                result.Data = null;
                return result;
            }
            result.StatusCode = 200;
            result.Message = "Create nutrition info OK!";
            result.Data = newInfo;
            return result;
        }

        //public async Task<ResponseObject<Nutrition>>
    }
}
