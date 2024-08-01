using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientCategoryModel
{
	public class ChangeStatusIngredientCategoryRequest
	{
		public BaseStatus Status { get; set; }
	}
}
