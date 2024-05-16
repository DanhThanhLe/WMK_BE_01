using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.UserModel
{
	public class UpdateUserRequest
	{
		public Guid Id { get; set; }
		public string? UserName { get; set; }
		public string? Email { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public Gender? Gender { get; set; }
		public string? Phone { get; set; }
		public string? Address { get; set; }
	}
}
