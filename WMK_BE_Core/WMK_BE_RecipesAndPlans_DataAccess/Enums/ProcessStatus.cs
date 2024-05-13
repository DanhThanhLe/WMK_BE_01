using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Enums
{
    public enum ProcessStatus
    {
        Processing = 0,
        Approved = 1,
        Denied = 2,
        Customer = 3,
        Cancel = 4,
    }

    public static class ProcessStatusHelper
    {
        public static int ToInt(this ProcessStatus status)
        {
            return (int)status;
        }
        //convert int to Status

        public static ProcessStatus FromInt(int value)
        {
            return Enum.IsDefined(typeof(ProcessStatus), value) ? (ProcessStatus)value : ProcessStatus.Processing;
        }
    }
}
