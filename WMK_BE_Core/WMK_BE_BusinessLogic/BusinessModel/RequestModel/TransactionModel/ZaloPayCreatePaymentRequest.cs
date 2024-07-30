using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel
{
	public class ZaloPayCreatePaymentRequest
	{
		public Guid OrderId { get; set; }
        public double Amount  { get; set; }

    }

    public class CreatePaymentRequest
    {
        public Guid OrderId { get; set; }
        public double Amount { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
