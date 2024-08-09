using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel
{
	public class RefundZaloPayRequest
	{
        public Guid IdOrder { get; set; }
        public Guid IdTransaction { get; set; }
        //public TransactionType TransactionType { get; set; }
        //public string MRefundId { get; set; }
		public string ZpTransId { get; set; }
		public string Description { get; set; }

	}
	public class ZaloPaySettings
	{
		public string AppId { get; set; }
		public string Key1 { get; set; }
		public string Key2 { get; set; }
		public string RefundUrl { get; set; }
	}

}
