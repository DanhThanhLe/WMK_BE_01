using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.CategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeAmountModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeNutrientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeStepModel.RecipeStep;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe
{
    public class RecipeResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ServingSize { get; set; }
        public int CookingTime { get; set; }
        public LevelOfDifficult Difficulty { get; set; }
        public string? Description { get; set; }
        public string? Notice { get; set; }
        public string? Img { get; set; }
        public double Price { get; set; }
        public int Popularity { get; set; }
        public string ProcessStatus { get; set; }
        public BaseStatus BaseStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; } = string.Empty;

        public List<RecipeIngredientResponse> RecipeIngredients { get; set; }
        public List<RecipeCategoryResponse> RecipeCategories { get; set; }
        public RecipeNutrientResponse RecipeNutrient { get; set; }
        public List<RecipeStepRespone> RecipeSteps { get; set; }
    }
}
