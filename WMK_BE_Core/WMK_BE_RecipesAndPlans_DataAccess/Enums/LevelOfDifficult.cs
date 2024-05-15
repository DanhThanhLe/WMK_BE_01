using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Enums
{
    public enum LevelOfDifficult
    {
        Normal = 0,
        Medinum = 1,
        Hard = 2
    }

    public static class LevelOfDifficultHelper
    {
		public static int ToInt(this LevelOfDifficult status)
		{
			return (int)status;
		}
		//convert int to Status

		public static LevelOfDifficult FromInt(int value)
		{
			return Enum.IsDefined(typeof(LevelOfDifficult) , value) ? (LevelOfDifficult)value : LevelOfDifficult.Normal;
		}
	}
}
