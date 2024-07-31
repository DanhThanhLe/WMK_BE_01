using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeStepModel.RecipeStep;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeModel
{
	public class UpdateRecipeRequest
	{
		public string? Name { get; set; } = string.Empty;
		public int? ServingSize { get; set; }
		public int? CookingTime { get; set; }
		public LevelOfDifficult? Difficulty { get; set; }
		public string? Description { get; set; } = string.Empty;
		public string? Img { get; set; } = string.Empty;
		public string UpdatedBy { get; set; } = string.Empty;

		public List<Guid>? CategoryIds { get; set; }
		public List<CreateRecipeIngredientRequest>? RecipeIngredientsList { get; set; }
		public List<CreateRecipeStepRequest>? Steps { get; set; }
	}
}
