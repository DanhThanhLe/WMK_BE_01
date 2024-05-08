using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
    [Table("NutritionInfos")]
    public class Nutrition
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Recipe))]
        public Guid RecipeID { get; set; }
        public double Calories { get; set; }
        public double Fat { get; set; }
        public double SaturatedFat { get; set; }
        public double Sugar { get; set; }
        public double Carbonhydrate { get; set; }
        public double DietaryFiber { get; set; }
        public double Protein { get; set; }
        public double Sodium { get; set; }


        //reference
        public virtual Recipe Recipe { get; set; }
    }
}
