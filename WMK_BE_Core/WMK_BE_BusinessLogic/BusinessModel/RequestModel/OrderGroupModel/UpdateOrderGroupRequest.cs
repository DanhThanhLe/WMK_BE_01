using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderGroupModel
{
	public class UpdateOrderGroupRequest
	{
        //public string Id { get; set; } = string.Empty;
        public string ShipperId { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public double Longitude { get; set; }//kinh dộ
		public double Latitude { get; set; }//vĩ độ
	}
}
