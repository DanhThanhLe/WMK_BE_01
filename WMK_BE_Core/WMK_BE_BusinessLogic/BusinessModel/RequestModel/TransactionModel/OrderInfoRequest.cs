using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel
{
	public class OrderInfoRequest
	{
		public Guid OrderId { get; set; }
		public Guid UserId { get; set; }
	}
}
