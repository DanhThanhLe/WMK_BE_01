using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.NutritionModel
{
    public class CreateNutritionRequest
    {
        //public Guid RecipeID { get; set; }
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
