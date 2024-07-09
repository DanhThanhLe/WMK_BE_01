using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface ITransactionService
	{
		Task<ResponseObject<MomoCreatePaymentRequest>> CreatePaymentAsync(OrderInfoRequest model);


	}
}
