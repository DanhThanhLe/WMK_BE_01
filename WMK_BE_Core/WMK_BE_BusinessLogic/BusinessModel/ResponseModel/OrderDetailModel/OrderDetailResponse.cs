using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeIngredientOrderDetailModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderDetailModel
{
    public class OrderDetailResponse
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid RecipeId { get; set; }
        public DayInWeek DayInWeek { get; set; }
        public MealInDay MealInDay { get; set; }
        public int Quantity { get; set; } //quantity cua recipe
        public double Price { get; set; } // = quantity * .Price

        public virtual RecipeResponse Recipe { get; set; }
        public List<RecipeIngredientOrderDetailResponse> RecipeIngredientOrderDetails {  get; set; }
        //recipe ingredient trong order detail
    }
}
