using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel
{
	public class GetAllOrdersRequest
	{
		public string ReceiveName { get; set; } = "";
		public string OrderCode { get; set; } = "";
		public DateTime ShipDate { get; set; }
		public DateTime OrderDate { get; set; }
	}
}
