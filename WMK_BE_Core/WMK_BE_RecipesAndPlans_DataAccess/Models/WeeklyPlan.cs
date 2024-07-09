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
        public DateTime? BeginDate { get; set; }//update after manager approve
        public DateTime? EndDate { get; set; }
        public string? Description { set; get; }
		public string? Notice { get; set; }

		public DateTime CreateAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public ProcessStatus ProcessStatus { get; set; }//thong tin ve viec duoc duyet hay chua.
                                                        //approve là đc duyet va co the hien thi tren app, deny hoac processing thi ko hien thi

        //reference
        public List<Order> Orders { get; set; }

        public List<RecipePLan> RecipePLans { get; set; }

    }
}
