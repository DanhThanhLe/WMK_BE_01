using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel
{
	public class MomoCreatePaymentRequest
	{
		public string partnerCode { get; set; } = string.Empty;
		public string orderId { get; set; } = string.Empty;
		public string requestId { get; set; } = string.Empty;
		public double amount { get; set; }
		public double responseTime { get; set; }
		public string message { get; set; } = string.Empty;
		public int resultCode { get; set; }
		public string payUrl { get; set; } = string.Empty;
		public string shortLink { get; set; } = string.Empty;
	}
}
