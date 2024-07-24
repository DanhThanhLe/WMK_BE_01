using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
    [Table("IngredientCategorys")]
    public class IngredientCategory                                                                 
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public BaseStatus Status { get; set; }


        //reference
        public virtual List<Ingredient> Ingredients { get; set; }

        public IngredientCategory()
        {
        }
    }
}
