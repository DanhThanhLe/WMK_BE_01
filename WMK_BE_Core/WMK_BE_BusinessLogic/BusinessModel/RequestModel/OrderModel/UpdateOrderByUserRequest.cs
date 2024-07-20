using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel
{
	public class UpdateOrderByUserRequest
	{
		public string Id { get; set; } = string.Empty;
		public string? Note { get; set; } = string.Empty;
		public string? Address { get; set; } = string.Empty;
		public double Longitude { get; set; }//kinh dộ
		public double Latitude { get; set; }//vĩ độ
	}
}
