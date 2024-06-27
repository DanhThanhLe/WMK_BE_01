using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{

    [Table("RecipeIngredients")]
    public class RecipeIngredient
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Recipe))]
        public Guid RecipeId { get; set; }
        [ForeignKey(nameof(Ingredient))]
        public Guid IngredientId { get; set; }
        public double Amount { get; set; }

        //reference
        public virtual Recipe Recipe { get; set; }
        public virtual Ingredient Ingredient { get; set; }

    }
}
