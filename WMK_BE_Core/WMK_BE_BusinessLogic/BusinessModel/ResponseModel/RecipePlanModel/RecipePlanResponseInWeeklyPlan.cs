using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipePlanModel
{
    public class RecipePlanResponseInWeeklyPlan
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public Guid StandardWeeklyPlanId { get; set; }

        public string DayInWeek { get; set; }
        public string MealInDay { get; set; }

        public int Quantity { get; set; }
        public double Price { get; set; }

        //reference
        public RecipeResponse Recipe { get; set; }
    }
}
