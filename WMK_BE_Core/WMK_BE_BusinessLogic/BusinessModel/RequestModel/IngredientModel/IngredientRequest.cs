﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel
{
    public class IngredientRequest//dung cho update ingredient
    {
        public Guid Id { get; set; }
        public Guid IngredientCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public double Price { get; set; }
        public BaseStatus Status { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; } = string.Empty;
    }

    public class UpdateStatusIngredientRequest
    {
        public Guid Id { get; set; }
        public BaseStatus Status { get; set; }

    }
}
