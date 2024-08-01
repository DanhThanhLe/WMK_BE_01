using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface
{
	public interface IUserRepository : IBaseRepository<User>
	{
		Task<User?> GetByEmailOrUserNameAsync(string emailOrUserName);
		Task<bool> GetOrderExistInUserAsync(Guid idUser);
		string GetUserNameById(Guid id);

    }
}
