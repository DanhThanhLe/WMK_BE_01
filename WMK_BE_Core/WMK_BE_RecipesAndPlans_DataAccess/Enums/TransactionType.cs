using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Enums
{
	public enum TransactionType
	{
		COD = 0,
		Momo = 1,
		ZaloPay = 2
	}
	public static class TransactionTypeHelper
	{
        public static int ToInt(this TransactionType type)
        {
            return (int)type;
        }
        //convert int to TransactionType
        public static TransactionType FromInt(int value)
        {
            return Enum.IsDefined(typeof(TransactionType), value) ? (TransactionType)value : TransactionType.COD;
        }
    }
}
