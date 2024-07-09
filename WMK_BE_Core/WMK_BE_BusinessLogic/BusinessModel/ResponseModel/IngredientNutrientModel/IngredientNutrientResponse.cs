using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientNutrientModel
{
    public class IngredientNutrientResponse
    {
        public Guid Id { get; set; }
        public Guid IngredientID { get; set; }
        public double Calories { get; set; }
        public double Fat { get; set; }
        public double SaturatedFat { get; set; }
        public double Sugar { get; set; }
        public double Carbonhydrate { get; set; }
        public double DietaryFiber { get; set; }
        public double Protein { get; set; }
        public double Sodium { get; set; }
    }
}
