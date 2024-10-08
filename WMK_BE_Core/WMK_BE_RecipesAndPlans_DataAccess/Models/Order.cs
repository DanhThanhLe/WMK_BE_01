﻿using System;
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
        public string? Note { get; set; }// lưu thông tin cần chú ý của đơn hàng
        public string? Message { get; set; }// thông báo của user hệ thống
		public string Address { get; set; } = string.Empty;
		public string? Img { get; set; }
		public DateTime ShipDate { get; set; }
		public DateTime OrderDate { get; set; }
		public double TotalPrice { get; set; }
		public double Longitude { get; set; }//kinh dộ
		public double Latitude { get; set; }//vĩ độ
		public string ReceiveName { get; set; }
		public string ReceivePhone { get; set; }
		public OrderStatus Status { get; set; }
		public string? ShipperNote { get; set; }
		public string? OrderTitle { get; set; }
		public string? OrderImg { get; set; }


		//reference
		public virtual User User { get; set; }
		public virtual OrderGroup? OrderGroup { get; set; }
		public virtual WeeklyPlan WeeklyPlan { get; set; }
		public virtual Transaction? Transaction { get; set; }
		public virtual Feedback FeedBacks { get; set; }

		//list
		public virtual List<OrderDetail> OrderDetails { get; set; }

        public Order()
        {
            
        }
    }
}
