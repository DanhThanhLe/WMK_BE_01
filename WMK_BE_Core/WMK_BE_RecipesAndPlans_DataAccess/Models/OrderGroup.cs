using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
	public class OrderGroup
	{
		public Guid Id { get; set; }
        [ForeignKey(nameof(User))]
        public Guid ShipperId { get; set; }

        public string Location { get; set; } = string.Empty;
        public DateTime AsignAt { get; set; }
        public Guid AsignBy { get; set; }


        public virtual User User { get; set; }

        public List<Order> Orders { get; set; }
    }
}
