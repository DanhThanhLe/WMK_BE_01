using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.TransactionModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface ITransactionService
	{
		Task<ResponseObject<List<TransactionResponse>>> GetAllAsync();

		//Task<ResponseObject<MomoCreatePaymentRequest>> CreatePaymentAsync(OrderInfoRequest model);
		Task<ResponseObject<TransactionResponse>> CreateNewPaymentAsync(CreatePaymentRequest request);

		#region ZaloPay
		Task<ResponseObject<Transaction>> CreatePaymentZaloPayAsync(ZaloPayCreatePaymentRequest model);
		Task<ResponseObject<Transaction>> UpdatePaymentZaloPayAsync(ZaloPayUpdatePaymentRequest model);
		#endregion

		#region Refund
		Task<ResponseObject<RefundZaloPayResponse>> RefundTransactionAsync(RefundZaloPayRequest request);
		#endregion

	}
}
