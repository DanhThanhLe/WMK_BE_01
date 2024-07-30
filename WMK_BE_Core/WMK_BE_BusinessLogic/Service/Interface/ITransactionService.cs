using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface ITransactionService
	{
		Task<ResponseObject<MomoCreatePaymentRequest>> CreatePaymentAsync(OrderInfoRequest model);
		Task<ResponseObject<Transaction>> CreateNewPaymentAsync(CreatePaymentRequest request);

        Task<ResponseObject<Transaction>> CreatePaymentZaloPayAsync(ZaloPayCreatePaymentRequest model);
		Task<ResponseObject<Transaction>> UpdatePaymentZaloPayAsync(ZaloPayUpdatePaymentRequest model);

	}
}
