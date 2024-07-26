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
    [Table("RecipeIngredientOrderDetails")]
    public class RecipeIngredientOrderDetail
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(OrderDetail))]
        public Guid OrderDetailId { get; set; }
        public Guid RecipeId { get; set; }
        public Guid IngredientId { get; set; }
        public double Amount { get; set; } //amount cua recipe trong RecipeIngredient
        public double IngredientPrice { get; set; } // = amount * .Price -> cai nay cua Ingredient

        //reference
        public virtual OrderDetail OrderDetail { get; set; }
        public virtual Ingredient Ingredient { get; set; }

        public RecipeIngredientOrderDetail()
        {
            
        }
    }
}
