using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
    public class RecipeCategory
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Category))]
        public Guid CategoryId {  get; set; }
        [ForeignKey(nameof(Recipe))]
        public Guid RecipeId { get; set; }

        //reference
        public virtual Recipe Recipe { get; set; }
        public virtual Category Category { get; set; }

    }
}
