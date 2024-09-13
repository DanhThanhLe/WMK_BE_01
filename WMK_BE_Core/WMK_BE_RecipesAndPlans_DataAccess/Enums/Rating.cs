using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Enums
{
	public enum Rating
	{
		VeryBad = 1,
		Bad = 2,
		Ok = 3,
		Good = 4,
		VeryGood = 5
	}
    public static class RatingHelper
    {
        public static int ToInt(this Rating rating)
        {
            return (int)rating;
        }
        //convert int to OrderStatus
        public static Rating FromInt(int value)
        {
            return Enum.IsDefined(typeof(Rating), value) ? (Rating)value : Rating.Ok;
        }
    }

}
