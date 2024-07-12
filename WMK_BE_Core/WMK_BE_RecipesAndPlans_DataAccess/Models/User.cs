using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
	[Table("Users")]
	public class User
	{
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public EmailConfirm EmailConfirm { get; set; }
		public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string? Phone { get; set; } = string.Empty;
        public Gender Gender { get; set; }
		public string? Address { get; set; }
		public string? Code { get; set; }
        public Role Role { get; set; }
        public int AccessFailedCount { get; set; }
        public BaseStatus Status { get; set; }


        public virtual OrderGroup OrderGroup { get; set; }

        //list
        public List<Order> Orders { get; set; }

        public User()
        {
            
        }
    }
}
