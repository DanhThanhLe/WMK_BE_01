using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
    [Table("WeeklyPlans")]
    public class WeeklyPlan
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { set; get; }
		public string? Notice { get; set; }
		public DateTime CreateAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; } = string.Empty;
        public ProcessStatus ProcessStatus { get; set; }

        //reference
        public List<Order> Orders { get; set; }

        public virtual List<RecipePLan> RecipePLans { get; set; }


        public WeeklyPlan()
        {
        }

    }
}
