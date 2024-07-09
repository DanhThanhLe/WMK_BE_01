using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel
{
	public class MomoOption
	{
		public string PaymentUrl { get; set; } = string.Empty;
		public string SecretKey { get; set; } = string.Empty;
		public string AccessKey { get; set; } = string.Empty;
		public string ReturnUrl { get; set; } = string.Empty;
		public string IpnUrl { get; set; } = string.Empty;
		public string PartnerCode { get; set; } = string.Empty;
		public string RequestType { get; set; } = string.Empty;
	}
}
