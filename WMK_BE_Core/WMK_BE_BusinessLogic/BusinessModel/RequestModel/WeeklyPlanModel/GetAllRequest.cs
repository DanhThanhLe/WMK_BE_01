using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel
{
	public class GetAllRequest
	{
		public string Title { get; set; } = "";
		public GetAllFilterDatimeRequest? DatetimeFilter { get; set; }
	}
	public class GetAllFilterDatimeRequest
	{
		public DateTime BeginDate { get; set; }
		public DateTime EndDate { get; set; }

	}
}
