using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Enums
{
	public enum TransactionStatus
	{
		PAID = 0,
		Pending = 1,
		RefundZaloPayPending = 2,
		RefundZaloPayDone = 3,
		UNPAID = 4,
	}
	public static class TransactionStatusHelper
	{
		public static int ToInt(this TransactionStatus status)
		{
			return (int)status;
		}
		public static TransactionStatus FromInt(int value)
		{
			return Enum.IsDefined(typeof(TransactionStatus) , value) ? (TransactionStatus)value : TransactionStatus.UNPAID;
		}
	}
}
