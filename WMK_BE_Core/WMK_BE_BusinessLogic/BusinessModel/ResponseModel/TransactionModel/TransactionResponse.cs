using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.TransactionModel
{
    public class TransactionResponse
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string Type { get; set; } //refield
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Notice { get; set; }
        public string? ExtraData { get; set; }
        public string? Signature { get; set; }
        public string Status { get; set; }
    }
}
