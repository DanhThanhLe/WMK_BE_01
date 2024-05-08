using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{

    [Table("RecipeAmounts")]
    public class RecipeAmount
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Recipe))]
        public Guid RecipeId { get; set; }
        [ForeignKey(nameof(Ingredient))]
        public Guid IngredientId { get; set; }
        public double Amount { get; set; }
        public double CurrentPricePerUnit { get; set; }//giu gia hien tai cua ingredient de luu vao thong tin order cung nhu tinh gia cua order do
                                                       //(gia order duoc tinh tu gia nguyen lieu cau thanh).
                                                       //luu o dang nay de phuc vu viec bao toan du lieu vi gia nguyen lieu thay doi lien tuc nen can luu lai gia cu
        //reference
        public virtual Recipe Recipe { get; set; }
        public virtual Ingredient Ingredient { get; set; }

    }
}
