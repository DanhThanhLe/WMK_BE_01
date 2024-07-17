using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.WeeklyPlanModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.CustomPlanModel
{
    public class CustomPlanResponse
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public Guid StandardWeeklyPlanId { get; set; }
        public DayInWeek DayInWeek { get; set; }
        public MealInDay MealInDay { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public RecipeResponse Recipe { get; set; }
        //public WeeklyPlanResponseModel WeeklyPlan { get; set; }
    }
}
