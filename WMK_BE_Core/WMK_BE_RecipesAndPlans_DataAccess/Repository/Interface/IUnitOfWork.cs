﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface
{
	public interface IUnitOfWork
	{
		ICategoryRepository CategoryRepository { get; }
		IWeeklyPlanRepository WeeklyPlanRepository { get; }

		Task CompleteAsync();
	}
}
