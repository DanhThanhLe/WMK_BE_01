using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Enums
{
    public enum DayInWeek
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sunday = 7
    }

    public static class DayInWeekHelper
    {
        public static int ToInt(this DayInWeek dayInWeek)
        {
            return (int)dayInWeek;
        }
        public static DayInWeek FromInt(int value)
        {
            return Enum.IsDefined(typeof(DayInWeek), value) ? (DayInWeek)value : DayInWeek.Monday;
        }
    }
}
