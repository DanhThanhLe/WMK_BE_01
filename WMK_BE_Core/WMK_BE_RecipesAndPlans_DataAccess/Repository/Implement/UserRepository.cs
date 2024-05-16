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
	public class UserRepository : BaseRepository<User>, IUserRepository
	{
        public UserRepository(WeMealKitContext context) : base(context)
        {
            
        }

		public async Task<bool> GetOrderExistInUserAsync(Guid idUser)
		{
			var userExist = await _dbSet.Include(u => u.Orders).FirstOrDefaultAsync(u => u.Id == idUser);
			if ( userExist != null && userExist.Orders.Count < 0 )
			{
				return true;
			}
			return false;
		}
	}
}
