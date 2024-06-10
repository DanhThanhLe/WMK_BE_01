using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderGroupModel
{
	public class CreateOrderGroupRequest
	{
		public Guid ShipperId { get; set; }
		public string Location { get; set; } = string.Empty;
		public Guid AsignBy { get; set; }
	}
}
