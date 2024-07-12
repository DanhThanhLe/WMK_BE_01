using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
	[Table("CustomPlans")]
	public class CustomPlan
	{
		[Key]
		public Guid Id { get; set; }
		[ForeignKey(nameof(Order))]
		public Guid OrderId { get; set; }
		public Guid RecipeId { get; set; }
		public Guid? StandardWeeklyPlanId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        //reference
        public virtual Order Order { get; set; }

        public CustomPlan()
        {
            
        }

    }
}
