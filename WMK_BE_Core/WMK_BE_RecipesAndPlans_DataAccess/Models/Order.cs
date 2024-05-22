using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
	[Table("Orders")]
	public class Order
	{
		[Key]
		public Guid Id { get; set; }
		[ForeignKey(nameof(User))]
		public Guid UserId { get; set; }
		[ForeignKey(nameof(WeeklyPlan))]
		public Guid? StanderdWeeklyPlanId { get; set; }//can null

		public string? Note { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
        public DateTime ShipDate { get; set; }
		public DateTime OrderDate { get; set; }
		public double TotalPrice { get; set; }
		public OrderStatus Status { get; set; }

		//reference
		public User User { get; set; }
		public WeeklyPlan WeeklyPlan { get; set; }

		//list
		public List<Feedback> FeedBacks { get; set; }
		public List<CustomPlan> CustomPlans { get; set; }
		public List<Transaction> Transactions { get; set; }
	}
}
