using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel
{
	public class CreateWeeklyPlanRequestModel
	{
		public string? Description { set; get; } = string.Empty;
		public string CreatedBy { get; set; } = string.Empty;
		public ProcessStatus ProcessStatus { get; set; }
		public List<Guid> recipesId { get; set; }
	}
}
