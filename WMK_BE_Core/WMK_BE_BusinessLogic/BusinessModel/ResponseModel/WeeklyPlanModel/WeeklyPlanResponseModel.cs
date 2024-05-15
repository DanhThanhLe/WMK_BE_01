using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.WeeklyPlanModel
{
	public class WeeklyPlanResponseModel
	{
		public Guid Id { get; set; }
		public DateTime BeginDate { get; set; }
		public DateTime EndDate { get; set; }
		public string? Description { set; get; } = string.Empty;
		public DateTime CreateAt { get; set; }
		public string CreatedBy { get; set; } = string.Empty;
		public DateTime? ApprovedAt { get; set; }
		public string? ApprovedBy { get; set; } = string.Empty;
		public DateTime? UpdatedAt { get; set; }
		public string? UpdatedBy { get; set; } = string.Empty;
		public string ProcessStatus { get; set; } = string.Empty;
		public List<WMK_BE_RecipesAndPlans_DataAccess.Models.Recipe> Recipes { get; set; }

	}
}
