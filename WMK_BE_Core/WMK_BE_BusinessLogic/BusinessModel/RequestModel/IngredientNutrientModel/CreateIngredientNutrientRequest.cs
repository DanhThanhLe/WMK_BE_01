using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientNutrientModel
{
    public class CreateIngredientNutrientRequest
    {
        public Guid IngredientID { get; set; }
        public double Calories { get; set; }
        public double Fat { get; set; }
        public double SaturatedFat { get; set; }
        public double Sugar { get; set; }
        public double Carbonhydrate { get; set; }
        public double DietaryFiber { get; set; }
        public double Protein { get; set; }
        public double Sodium { get; set; }

        //public Ingredient Ingredient { get; set; }
    }
}
