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
		PendingCOD = 1,//pending chung,
		PendingMomo = 2,
		PendingZaloPay = 3,
		RefundZaloPayPending = 4,
		RefundZaloPayDone = 5,
		UNPAID = 6,
		Cancel = 7,
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
