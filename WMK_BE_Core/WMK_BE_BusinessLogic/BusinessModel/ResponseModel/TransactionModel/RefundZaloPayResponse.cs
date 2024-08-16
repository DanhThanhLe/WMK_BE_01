using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.TransactionModel
{
	public class RefundZaloPayResponse
	{
		[JsonProperty("return_code")]
		public int ReturnCode { get; set; }

		[JsonProperty("return_message")]
		public string ReturnMessage { get; set; } = string.Empty;

		[JsonProperty("sub_return_code")]
		public int SubReturnCode { get; set; }

		[JsonProperty("sub_return_message")]
		public string SubReturnMessage { get; set; } = string.Empty;
		public string EmailCustomer { get; set; } = string.Empty;
		public string OrderCode { get; set; } = string.Empty;
	}
}
