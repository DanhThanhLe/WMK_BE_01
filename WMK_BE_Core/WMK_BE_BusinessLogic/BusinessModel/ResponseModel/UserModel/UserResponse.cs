using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.UserModel
{
	public class UsersResponse
	{
		public Guid Id { get; set; }
		public string Email { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string EmailConfirm { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Gender { get; set; } = string.Empty;
		public DateTime? DateOfBirth { get; set; }
		public string? Address { get; set; }
		public string Role { get; set; } = string.Empty;
		public int AccessFailedCount { get; set; }
		public string Status { get; set; } = string.Empty;
	}
	
	public class UserResponse
	{
		public Guid Id { get; set; }
		public string Email { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Gender { get; set; } = string.Empty;
		public DateTime? DateOfBirth { get; set; }
		public string? Address { get; set; }
		public string Role { get; set; } = string.Empty;
	}
}
