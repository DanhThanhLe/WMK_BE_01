using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public ProcessStatus ProcessStatus { get; set; }
		public List<RecipeAmountModel> Ingredients { get; set; }
		public List<Guid> CategoryIds { get; set; }
	}
    public class RecipeAmountModel
    {
        public Guid IngredientId { get; set; }
        public int amount { get; set; }
    }
}
