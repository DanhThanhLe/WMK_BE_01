using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel
{
	public class ZaloPayCreatePaymentRequest
	{
		public Guid OrderId { get; set; }
        public double Amount  { get; set; }

    }

    public class CreatePaymentRequest //model nay dung chung cho khi update thong tin tracsaction cho order (vi order co 1 list transaction nen khi update status transaction thi se tao mot transaction voi status moi
    {
        public Guid OrderId { get; set; }
        public TransactionType Type { get; set; }//loai hinh thanh toan
        public double Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? Notice { get; set; }
        public string? ExtraData { get; set; }
        public string? Signature { get; set; }
        public TransactionStatus? Status { get; set; }//trang thai cho transaction
    }
}
