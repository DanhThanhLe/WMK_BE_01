using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
	public class BankingInfo
	{
		[Key]
		public Guid Id { get; set; }
		public string NameBanking { get; set; } = string.Empty;
		public int Status { get; set; }


		//list
		public virtual List<Transaction> Transactions { get; set; }
	}
}
