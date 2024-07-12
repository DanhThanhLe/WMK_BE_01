using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.NutritionModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeStepModel.RecipeStep;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe
{
    public class CreateRecipeRequest
    {
        public string Name { get; set; } = string.Empty;
        public int ServingSize { get; set; }
        public LevelOfDifficult Difficulty { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string? ImageLink { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;

        public List<Guid> CategoryIds { get; set; }
		public List<CreateRecipeIngredientRequest> RecipeIngredientsList { get; set; }
        public List<CreateRecipeStepRequest> Steps { get; set; }
	}
    public class CreateRecipeIngredientRequest
    {
        public Guid IngredientId { get; set; }
        public double amount { get; set; }
    }
}
