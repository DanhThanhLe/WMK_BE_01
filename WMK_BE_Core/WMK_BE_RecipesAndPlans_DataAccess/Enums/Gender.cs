using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Enums
{
	public enum Gender
	{
		Male = 0,
		Female = 1,
		Non = 2
	}
	public static class UserGenderHelper
	{
		public static int ToInt(this Gender gender)
		{
			return (int)gender;
		}
		public static Gender FromInt(int value)
		{
			return Enum.IsDefined(typeof(Gender) , value) ? (Gender)value : Gender.Non;
		}
	}
}
