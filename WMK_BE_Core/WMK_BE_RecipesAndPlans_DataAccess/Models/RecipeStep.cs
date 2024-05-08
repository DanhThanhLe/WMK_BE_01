using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
    [Table("RecipeSteps")]
    public class RecipeStep
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Recipe))]
        public Guid RecipeId { get; set; }
        public int Index { get; set; }
        public string? MediaURL { get; set; }
        public string? ImageLink { get; set; }
        public string? Description { get; set; }

        //reference
        public virtual Recipe Recipe { get; set; }
    }
}
