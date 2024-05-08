using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
    [Table("Ingredients")]
    public  class Ingredient
    {
        [Key]
        public Guid Id { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public double? PricebyUnit { get; set; }
        public string Unit { get; set; }
        public BaseStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }


        //reference
        public List<RecipeAmount> RecipeAmounts { get; set;}


    }
}
