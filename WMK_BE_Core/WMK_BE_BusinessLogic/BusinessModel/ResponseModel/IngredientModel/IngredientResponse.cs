﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientNutrientModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientModel
{
	public class IngredientResponse
	{
		public Guid Id { get; set; }
		public Guid IngredientCategoryId { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Img { get; set; } = string.Empty;
		public string Unit { get; set; } = string.Empty;
		public double Price { get; set; }
        public string PackagingMethod { get; set; }
        public string PreservationMethod { get; set; }
        public string Status { get; set; }
		public DateTime CreatedAt { get; set; }
		public string CreatedBy { get; set; } = string.Empty;

		public IngredientNutrientResponse IngredientNutrient { get; set; }
		public IngredientCategoryResponse IngredientCategory { get; set; }
	}
}
