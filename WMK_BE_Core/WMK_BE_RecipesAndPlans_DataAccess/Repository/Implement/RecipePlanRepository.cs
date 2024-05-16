using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
	public class RecipePlanRepository : BaseRepository<RecipePLan>, IRecipePlanRepository
	{
        public RecipePlanRepository(RecipesAndPlansContext context) : base (context)
        {
            
        }

		public async Task AddRangeAsync(IEnumerable<RecipePLan> recipePlans)
		{
			await _dbSet.AddRangeAsync(recipePlans);
		}

		public async Task<List<RecipePLan>> GetListByPlanIdAsync(Guid planId)
		{
			return await _dbSet
				.Where(rp => rp.StandardWeeklyPlanId == planId)
				.ToListAsync();
		}

		public void RemoveRange(IEnumerable<RecipePLan> recipePlans)
		{
			_dbSet.RemoveRange(recipePlans);
		}
	}
}
