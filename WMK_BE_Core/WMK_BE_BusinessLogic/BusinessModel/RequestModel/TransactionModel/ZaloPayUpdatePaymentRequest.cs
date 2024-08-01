using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel
{
	public class ZaloPayUpdatePaymentRequest
	{
        public Guid Id { get; set; }
        public TransactionStatus Status { get; set; }
    }
}
