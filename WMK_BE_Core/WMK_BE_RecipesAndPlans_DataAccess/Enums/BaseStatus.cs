using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Enums
{
    public enum BaseStatus
    {
        Available = 0,
        UnAvailable = 1
    }

    public static class BaseStatusHelper
    {
        public static int ToInt(this BaseStatus status)
        {
            return (int) status;
        }
        //convert int to Status

        public static BaseStatus FromInt(int value)
        {
            return Enum.IsDefined(typeof(BaseStatus), value) ? (BaseStatus)value : BaseStatus.UnAvailable;
        }
    }
}
