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
		PendingMomo = 1,
		PendingZaloPay = 2,
		UNPAID = 3,
		Cancel = 4,
		Pending = 5,//pending chung,
		RefundZaloPayPending = 7,
		RefundZaloPayDone = 8,
	}
	public static class TransactionStatusHelper
	{
		public static int ToInt(this TransactionStatus status)
		{
			return (int)status;
		}
		public static TransactionStatus FromInt(int value)
		{
			return Enum.IsDefined(typeof(TransactionStatus) , value) ? (TransactionStatus)value : TransactionStatus.Cancel;
		}
	}
}
