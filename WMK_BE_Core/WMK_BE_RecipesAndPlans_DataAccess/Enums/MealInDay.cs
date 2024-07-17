using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Enums
{
    public enum MealInDay
    {
        Breakfast = 1,
        Lunch = 2,
        Dinner = 3
    }

    public static class MealInDayHelper
    {
        public static int ToInt(this MealInDay mealInDay)
        {
            return (int)mealInDay;
        }
        public static MealInDay FromInt(int value)
        {
            return Enum.IsDefined(typeof(MealInDay), value) ? (MealInDay)value : MealInDay.Breakfast;
        }
    }
}
