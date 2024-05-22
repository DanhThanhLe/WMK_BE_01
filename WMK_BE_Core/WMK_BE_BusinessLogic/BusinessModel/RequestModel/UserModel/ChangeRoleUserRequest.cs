using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.UserModel
{
	public class ChangeRoleUserRequest
	{
		public string Id { get; set; } = string.Empty;
		public Role NewRole { get; set; }
	}
}
