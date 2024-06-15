using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderGroupModel
{
	public class OrderGroupsResponse
	{
		public Guid Id { get; set; }
		public Guid ShipperId { get; set; }
		public string Location { get; set; } = string.Empty;
        public double[]? Coordinates { get; set; }
        public DateTime AsignAt { get; set; }
		public Guid AsignBy { get; set; }
	}
}
