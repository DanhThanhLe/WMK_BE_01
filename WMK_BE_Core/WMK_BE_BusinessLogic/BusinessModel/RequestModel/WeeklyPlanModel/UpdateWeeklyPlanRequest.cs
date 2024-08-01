using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipePlanModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel
{
    public class UpdateWeeklyPlanRequest//model nay dung cho viec cap nhat thong tin toan phan danh cho weeklyPlan nhat dinh, ap dung cho ca web admin va ca customer
    {
        //public Guid Id { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? UrlImage { get; set; } = string.Empty;
        public string? Title { get; set; } = string.Empty;
        public string? Description { set; get; } = string.Empty;
        public string? Notice { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; } = string.Empty;
        public ProcessStatus? ProcessStatus { get; set; }
        public List<RecipeWeeklyPlanCreate>? recipeIds { get; set; }
    }
}
