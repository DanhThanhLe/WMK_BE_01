﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using TransactionStatus = WMK_BE_RecipesAndPlans_DataAccess.Enums.TransactionStatus;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
	[Table("Transactions")]
	public class Transaction
	{
		[Key]
		public string Id { get; set; } = string.Empty;
		[ForeignKey(nameof(Order))]
		public Guid OrderId { get; set; }
		[ForeignKey(nameof(BankingInfo))]
		public Guid BankingInfoId { get; set; }
		public TransactionType Type { get; set; } //refield

		public double Amount { get; set; }
		public DateTime TransactionDate { get; set; }
		public string? Notice { get; set; }
		public string? ExtraData { get; set; }
		public string? Signature { get; set; }
		public TransactionStatus Status { get; set; }


		public virtual BankingInfo BankingInfo { get; set; }
		public virtual Order Order { get; set; }
	}
}
