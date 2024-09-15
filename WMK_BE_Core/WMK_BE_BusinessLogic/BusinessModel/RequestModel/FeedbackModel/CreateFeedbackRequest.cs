using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.FeedbackModel
{
    public class CreateFeedbackRequest //model dung de tao feedback tu order cua khach hang
    {
        //public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Rating Rating { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
    }
}
