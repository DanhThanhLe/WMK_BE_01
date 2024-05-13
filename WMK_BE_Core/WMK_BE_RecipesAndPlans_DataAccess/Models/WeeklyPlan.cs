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
        public string? Description { set; get; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; } = string.Empty;
        public int Popularity { get; set; }//chi so dung de do luong do ua chuong cua plan dua vao luot dat hang
        public ProcessStatus ProcessStatus { get; set; }//thong tin ve viec duoc duyet hay chua.
                                                        //approve là đc duyet va co the hien thi tren app, deny hoac processing thi ko hien thi

        public List<RecipePLan> RecipePLans { get; set; }

    }
}
