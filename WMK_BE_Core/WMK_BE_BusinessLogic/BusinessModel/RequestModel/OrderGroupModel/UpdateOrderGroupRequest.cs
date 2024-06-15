using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderGroupModel
{
	public class UpdateOrderGroupRequest
	{
        public Guid Id { get; set; }
        public Guid ShipperId { get; set; }
		public string Location { get; set; } = string.Empty;
	}
}
