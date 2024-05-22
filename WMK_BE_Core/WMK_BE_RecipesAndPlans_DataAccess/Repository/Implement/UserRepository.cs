using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
	public class UserRepository : BaseRepository<User>, IUserRepository
	{
        public UserRepository(WeMealKitContext context) : base(context)
        {
            
        }

		public override Task<bool> DeleteAsync(string id)
		{
			return base.DeleteAsync(id);
		}

		public async Task<User?> GetByEmailOrUserNameAsync(string emailOrUserName)
		{
			var userExist = await _dbSet.FirstOrDefaultAsync(u => u.Email == emailOrUserName || u.UserName == emailOrUserName);
            if (userExist != null)
            {
				return userExist;
            }
			return null;
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
