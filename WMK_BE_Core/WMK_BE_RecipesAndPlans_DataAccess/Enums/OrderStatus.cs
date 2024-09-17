using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Enums
{
	public enum OrderStatus
	{
		Processing = 0,
		Confirm = 1,
		Shipping = 2,
		Shipped = 3,
		UnShipped = 4,
		Delivered = 5,
		Refund = 6,
		Canceled = 7
	}
	public static class OrderStatusHelper
	{
		public static int ToInt(this OrderStatus status)
		{
			return (int)status;
		}
		//convert int to OrderStatus
		public static OrderStatus FromInt(int value)
		{
			return Enum.IsDefined(typeof(OrderStatus), value) ? (OrderStatus)value : OrderStatus.Canceled;
		}
	}
}
