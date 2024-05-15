using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel
{
	public class UpdateWeeklyPlanRequestModel
	{
		public Guid Id { get; set; }
		public DateTime? BeginDate { get; set; }//update after manager approve
		public DateTime? EndDate { get; set; }
		public string? Description { set; get; } = string.Empty;
		public string? UpdatedBy { get; set; } = string.Empty;
		public List<Guid> recipeIds { get; set; }
	}
}
