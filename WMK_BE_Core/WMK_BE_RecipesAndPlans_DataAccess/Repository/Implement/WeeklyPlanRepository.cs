﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
	public class WeeklyPlanRepository: BaseRepository<WeeklyPlan>, IWeeklyPlanRepository
	{
        public WeeklyPlanRepository(WeMealKitContext context) : base(context)
        {
            
        }

		public override Task<List<WeeklyPlan>> GetAllAsync()
		{
			return _dbSet.Include(wp => wp.RecipePLans)
				.ThenInclude(rp => rp.Recipe)
				.ToListAsync();
		}
		public override async Task<WeeklyPlan?> GetByIdAsync(string id)
		{
			try
			{
				Guid guidId;
				if ( !Guid.TryParse(id , out guidId) )
				{
					return null;
				}
				var entity = await _dbSet.Include(wp => wp.RecipePLans)
									.ThenInclude(rp => rp.Recipe)
									.FirstOrDefaultAsync(wp => wp.Id == guidId);
				return entity;
			}
			catch ( Exception ex )
			{
				Console.WriteLine($"Error occurred in GetAsync: {ex}");
				return null;
			}
		}

		public async Task<bool> RecipeExistInWeeklyPlanAsync(Guid weeklyPlanId)
		{
			var weeklyplan = await _dbSet.Include(w => w.RecipePLans).FirstOrDefaultAsync(w => w.Id == weeklyPlanId);
            if (weeklyplan != null)
            {
                return true;
            }
            return false;
        }
	}
}
