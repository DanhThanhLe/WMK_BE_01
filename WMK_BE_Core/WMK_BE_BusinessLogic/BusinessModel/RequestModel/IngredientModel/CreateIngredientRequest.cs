using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientNutrientModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel
{
    public class CreateIngredientRequest
    {
        public Guid IngredientCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public double Price { get; set; }
        public string PackagingMethod { get; set; }
        public string PreservationMethod { get; set; }
        public BaseStatus Status { get; set; }
        public string CreatedBy { get; set; } = string.Empty;//Ten nguoi tao

        public CreateIngredientNutrientRequest NutrientInfo { get; set; }

    }
}
