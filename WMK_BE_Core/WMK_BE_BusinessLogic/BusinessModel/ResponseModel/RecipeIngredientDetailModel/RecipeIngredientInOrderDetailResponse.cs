using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeIngredientDetailModel
{
    public class RecipeIngredientInOrderDetailResponse
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(OrderDetail))]
        public Guid OrderDetailId { get; set; }
        public Guid RecipeId { get; set; }
        public Guid IngredientId { get; set; }
        public double Amount { get; set; } //amount cua recipe trong RecipeIngredient
        public double IngredientPrice { get; set; } // = amount * .Price -> cai nay cua Ingredient
        public virtual OrderDetail OrderDetail { get; set; }
    }
}
