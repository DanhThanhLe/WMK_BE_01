using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using System.Text.Json;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
	[Table("Orders")]
	public class Order
	{
		[Key]
		public Guid Id { get; set; }
		[ForeignKey(nameof(User))]
		public Guid UserId { get; set; }
		[ForeignKey(nameof(OrderGroup))]
		public Guid? OrderGroupId { get; set; }
		[ForeignKey(nameof(WeeklyPlan))]
		public Guid? StanderdWeeklyPlanId { get; set; }

        public int OrderCode { get; set; }
        public string? Note { get; set; }
		public string Address { get; set; } = string.Empty;
		public string? Img { get; set; }
		public DateTime ShipDate { get; set; }
		public DateTime OrderDate { get; set; }
		public double TotalPrice { get; set; }
		public double Longitude { get; set; }//kinh dộ
		public double Latitude { get; set; }//vĩ độ
		public OrderStatus Status { get; set; }

		//reference
		public virtual User User { get; set; }
		public virtual OrderGroup OrderGroup { get; set; }
		public virtual WeeklyPlan WeeklyPlan { get; set; }

		//list
		public List<Feedback> FeedBacks { get; set; }
		public List<OrderDetail> OrderDetails { get; set; }
		public List<Transaction> Transactions { get; set; }// cho cọc trước

        public Order()
        {
            
        }
    }
}
