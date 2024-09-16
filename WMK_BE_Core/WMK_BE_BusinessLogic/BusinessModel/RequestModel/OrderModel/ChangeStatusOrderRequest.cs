using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel
{
	public class ChangeStatusOrderRequest
	{
        //public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public string? ShipperNote { get; set; }
        public string? Img { get; set; }
        //public int? rating { get; set; }

    }
}
