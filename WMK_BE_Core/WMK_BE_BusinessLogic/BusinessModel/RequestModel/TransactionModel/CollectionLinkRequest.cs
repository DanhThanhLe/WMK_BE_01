using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel
{
	public class CollectionLinkRequest
	{
		public string orderInfo { get; set; } = string.Empty;
		public string partnerCode { get; set; } = string.Empty;
		public string redirectUrl { get; set; } = string.Empty;
		public string ipnUrl { get; set; } = string.Empty;
		public long amount { get; set; }
		public string orderId { get; set; } = string.Empty;
		public string requestId { get; set; } = string.Empty;
		public string extraData { get; set; } = string.Empty;
		public string partnerName { get; set; } = string.Empty;
		public string storeId { get; set; } = string.Empty;
		public string requestType { get; set; } = string.Empty;
		public string orderGroupId { get; set; } = string.Empty;
		public bool autoCapture { get; set; }
		public string lang { get; set; } = string.Empty;
		public string signature { get; set; } = string.Empty;
	}
}
