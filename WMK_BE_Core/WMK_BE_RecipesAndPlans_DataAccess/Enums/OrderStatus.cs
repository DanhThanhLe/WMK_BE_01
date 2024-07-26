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
		Shipping = 1,
		Shipped = 2,
		Delivered = 3,
		UnShipped = 4,
		Refund = 5,
		Canceled = 6
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
