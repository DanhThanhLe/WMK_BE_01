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
        [ForeignKey(nameof(IngredientCategory))]
        public Guid IngredientCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string? Img { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public BaseStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; } = string.Empty;


        //reference
        public virtual IngredientCategory IngredientCategory { get; set; }
        public virtual IngredientNutrient IngredientNutrient { get; set; }
        public virtual List<RecipeIngredient> RecipeIngredients { get; set;}

        public Ingredient()
        {
            RecipeIngredients = new List<RecipeIngredient>();
        }

    }
}
