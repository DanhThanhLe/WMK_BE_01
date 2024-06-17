using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Enums
{
	public enum Role
	{
		Admin = 0,
		Manager = 1,
		Staff = 2,
		Shipper = 3,
		Customer = 4,
		None = 5,
	}
	public static class RoleHelper
	{
		public static int ToInt(this Role role)
		{
			return (int)role;
		}
		public static Role FromInt(int value)
		{
			return Enum.IsDefined(typeof(Role) , value) ? (Role)value : Role.None;
		}
	}
}
