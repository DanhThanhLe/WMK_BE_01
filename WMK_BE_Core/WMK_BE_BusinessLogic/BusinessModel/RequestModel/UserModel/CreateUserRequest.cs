using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.UserModel
{
	public class CreateUserRequest
	{
		public string Email { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public Gender Gender { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? Phone { get; set; }
		public string? Address { get; set; }
		public Role Role { get; set; }
	}
}
