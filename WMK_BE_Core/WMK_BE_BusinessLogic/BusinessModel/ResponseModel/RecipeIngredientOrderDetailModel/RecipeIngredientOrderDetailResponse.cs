using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeIngredientOrderDetailModel
{
    public class RecipeIngredientOrderDetailResponse
    {
        public Guid Id { get; set; }
        public Guid OrderDetailId { get; set; }
        public Guid RecipeId { get; set; }
        public Guid IngredientId { get; set; }
        public double Amount { get; set; } //amount cua recipe trong RecipeIngredient
        public double IngredientPrice { get; set; } // = amount * .Price -> cai nay cua Ingredient
        public IngredientResponse Ingredient {  get; set; }
    }
}
