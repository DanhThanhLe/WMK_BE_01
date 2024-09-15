using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
	[Table("Feedbacks")]
	public class Feedback
	{
		[Key]
		public Guid Id { get; set; }
		[ForeignKey(nameof(Order))]
		public Guid OrderId { get; set; }

		public Rating Rating { get; set; }
		public string? Description { get; set; }
		//public IsOrder IsOrder { get; set; }

		public DateTime CreatedAt { get; set; }
		public string CreatedBy { get; set; } = string.Empty;

		//reference
		public virtual Order Order { get; set; }

        public Feedback()
        {
            
        }
    }
}
