using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderModel
{
	public class BaseOrderResponse
	{
		public Guid Id { get; set; }
		public Guid? UserId { get; set; }
	}
}
