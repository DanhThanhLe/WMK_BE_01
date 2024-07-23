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
	[Table("OrderDetails")] 
	public class OrderDetail
	{
		[Key]
		public Guid Id { get; set; }
        [ForeignKey(nameof(Order))]
        public Guid OrderId { get; set; }
		public Guid RecipeId { get; set; }
        public DayInWeek DayInWeek { get; set; }
        public MealInDay MealInDay { get; set; }
        public int Quantity { get; set; } //quantity cua recipe
        public double Price { get; set; } // = quantity * .Price



        //reference
        public virtual Order Order { get; set; }
        public virtual List<RecipeIngredientOrderDetail> RecipeIngredientOrderDetails { get; set; }
        //public virtual Recipe Recipe { get; set; }//quan he 1 - nhieu (recipe - customPlan)
        //public virtual WeeklyPlan WeeklyPlan { get; set; }//quan he 1 - nhieu (WeeklyPlan - customPlan)

        public OrderDetail()
        {
            
        }

    }
}
