using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel;
using WMK_BE_BusinessLogic.Helpers;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class TransactionService : ITransactionService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IOptions<MomoOption> _momoOptions;

		public TransactionService(IUnitOfWork unitOfWork, IOptions<MomoOption> momoOptions)
        {
            _unitOfWork = unitOfWork;
			_momoOptions = momoOptions;
        }

		public async Task<ResponseObject<MomoCreatePaymentRequest>> CreatePaymentAsync(OrderInfoRequest model)
		{
			var result = new ResponseObject<MomoCreatePaymentRequest>();
			//check order exist
			var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(model.OrderId.ToString());
			if ( orderExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Order does not exist!";
				return result;
			}
			var transactionExist = orderExist.Transactions;
			if ( transactionExist != null )
			{
				foreach(var transaction in transactionExist )
				{
					if(transaction.Status == WMK_BE_RecipesAndPlans_DataAccess.Enums.TransactionStatus.PAID )
					{
						break;
					}
				}
				result.StatusCode = 300;
				result.Message = "Order is payed!";
				return result;
			}
			//check user exist in order
			var userExist = await _unitOfWork.OrderRepository.GetUserExistInOrderAsync(model.OrderId , model.UserId);
			if ( userExist == false )
			{
				result.StatusCode = 400;
				result.Message = "User does not match with order!";
				return result;
			}
			//create new request id
			var requestId = Guid.NewGuid();
			var orderInfo = "Order from: " + orderExist.User.FirstName + orderExist.User.LastName;
			//create model request
			CollectionLinkRequest request = new CollectionLinkRequest();
			request.orderInfo = orderInfo;
			request.partnerCode = _momoOptions.Value.PartnerCode;
			request.ipnUrl = _momoOptions.Value.IpnUrl;
			request.redirectUrl = _momoOptions.Value.ReturnUrl;
			if ( orderExist.TotalPrice <= 0 )
			{
				result.StatusCode = 404;
				result.Message = "Order price is null!";
				return result;
			}
			request.amount = (long)orderExist.TotalPrice;
			request.orderId = orderExist.Id.ToString();
			request.requestId = requestId.ToString();
			request.requestType = _momoOptions.Value.RequestType;
			request.extraData = model.UserId.ToString();
			// request.partnerName = "WEMEALKIT";
			// request.storeId = "Test Store";
			// request.orderGroupId = "";
			request.autoCapture = true;
			request.lang = "vi";
			var rawSignature = "accessKey=" + _momoOptions.Value.AccessKey
				+ "&amount=" + request.amount
				+ "&extraData=" + request.extraData
				+ "&ipnUrl=" + request.ipnUrl
				+ "&orderId=" + request.orderId
				+ "&orderInfo=" + request.orderInfo
				+ "&partnerCode=" + request.partnerCode
				+ "&redirectUrl=" + request.redirectUrl
				+ "&requestId=" + request.requestId
				+ "&requestType=" + request.requestType;
			request.signature = HashHelper.GetSignature256(rawSignature , _momoOptions.Value.SecretKey);
			StringContent httpContent = new StringContent(JsonSerializer.Serialize(request) , System.Text.Encoding.UTF8 , "application/json");
			var client = new HttpClient();
			var quickPayResponse = await client.PostAsync(_momoOptions.Value.PaymentUrl , httpContent);
			var contents = quickPayResponse.Content.ReadAsStringAsync().Result;
			var deserialize = JsonSerializer.Deserialize<MomoCreatePaymentRequest>(contents);
			if ( deserialize != null )
			{
				result.StatusCode = deserialize.resultCode;
				result.Message = deserialize.message;
				result.Data = JsonSerializer.Deserialize<MomoCreatePaymentRequest>(contents);
				return result;
			}
			else
			{
				result.StatusCode = 400;
				result.Message = "Fail call to MoMo!";
				return result;
			}
		}
	}
}
