﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel
{
	public class ChangeBaseStatusWeeklyPlanRequest
	{
		public BaseStatus BaseStatus { get; set; }
	}
}
