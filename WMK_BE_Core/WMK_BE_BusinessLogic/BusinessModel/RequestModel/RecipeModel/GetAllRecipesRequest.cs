using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeModel
{
	public class GetAllRecipesRequest
	{
		public string Name { get; set; } = "";
		public int? ServingSize { get; set; } 
		public int? CookingTime { get; set; }
		public LevelOfDifficult? Difficulty { get; set; }
		public string Description { get; set; } = "";
	}
}
