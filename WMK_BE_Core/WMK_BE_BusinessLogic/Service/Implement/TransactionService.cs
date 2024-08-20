using AutoMapper;
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
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.TransactionModel;
using System.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class TransactionService : ITransactionService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IOptions<MomoOption> _momoOptions;
		private readonly IOptions<ZaloPaySettings> _zaloPayOptions;
		private readonly IMapper _mapper;
		private readonly CreateZaloPayValidator _createZaloPayValidator;
		private readonly CreateTransactionValidator _createTransactionValidator;
		public TransactionService(IUnitOfWork unitOfWork , IOptions<MomoOption> momoOptions , IOptions<ZaloPaySettings> zaloPayOptions , IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_momoOptions = momoOptions;
			_zaloPayOptions = zaloPayOptions;
			_mapper = mapper;
			_createZaloPayValidator = new CreateZaloPayValidator();
			_createTransactionValidator = new CreateTransactionValidator();
		}


		#region Create momo - tao rieng cho momo - chua kiem thu
		//public async Task<ResponseObject<MomoCreatePaymentRequest>> CreatePaymentAsync(OrderInfoRequest model)
		//{
		//    var result = new ResponseObject<MomoCreatePaymentRequest>();
		//    //check order exist
		//    var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(model.OrderId.ToString());
		//    if (orderExist == null)
		//    {
		//        result.StatusCode = 404;
		//        result.Message = "Order does not exist!";
		//        return result;
		//    }
		//    var transactionExist = orderExist.Transactions;
		//    if (transactionExist != null)
		//    {
		//        foreach (var transaction in transactionExist)
		//        {
		//            if (transaction.Status == TransactionStatus.PAID)
		//            {
		//                break;
		//            }
		//        }
		//        result.StatusCode = 300;
		//        result.Message = "Order is payed!";
		//        return result;
		//    }
		//    //check user exist in order
		//    var userExist = await _unitOfWork.OrderRepository.GetUserExistInOrderAsync(model.OrderId, model.UserId);
		//    if (userExist == false)
		//    {
		//        result.StatusCode = 400;
		//        result.Message = "User does not match with order!";
		//        return result;
		//    }
		//    //create new request id
		//    var requestId = Guid.NewGuid();
		//    var orderInfo = "Order from: " + orderExist.User.FirstName + orderExist.User.LastName;
		//    //create model request
		//    CollectionLinkRequest request = new CollectionLinkRequest();
		//    request.orderInfo = orderInfo;
		//    request.partnerCode = _momoOptions.Value.PartnerCode;
		//    request.ipnUrl = _momoOptions.Value.IpnUrl;
		//    request.redirectUrl = _momoOptions.Value.ReturnUrl;
		//    if (orderExist.TotalPrice <= 0)
		//    {
		//        result.StatusCode = 404;
		//        result.Message = "Order price is null!";
		//        return result;
		//    }
		//    request.amount = (long)orderExist.TotalPrice;
		//    request.orderId = orderExist.Id.ToString();
		//    request.requestId = requestId.ToString();
		//    request.requestType = _momoOptions.Value.RequestType;
		//    request.extraData = model.UserId.ToString();
		//    // request.partnerName = "WEMEALKIT";
		//    // request.storeId = "Test Store";
		//    // request.orderGroupId = "";
		//    request.autoCapture = true;
		//    request.lang = "vi";
		//    var rawSignature = "accessKey=" + _momoOptions.Value.AccessKey
		//        + "&amount=" + request.amount
		//        + "&extraData=" + request.extraData
		//        + "&ipnUrl=" + request.ipnUrl
		//        + "&orderId=" + request.orderId
		//        + "&orderInfo=" + request.orderInfo
		//        + "&partnerCode=" + request.partnerCode
		//        + "&redirectUrl=" + request.redirectUrl
		//        + "&requestId=" + request.requestId
		//        + "&requestType=" + request.requestType;
		//    request.signature = HashHelper.GetSignature256(rawSignature, _momoOptions.Value.SecretKey);
		//    StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
		//    var client = new HttpClient();
		//    var quickPayResponse = await client.PostAsync(_momoOptions.Value.PaymentUrl, httpContent);
		//    var contents = quickPayResponse.Content.ReadAsStringAsync().Result;
		//    var deserialize = JsonSerializer.Deserialize<MomoCreatePaymentRequest>(contents);
		//    if (deserialize != null)
		//    {
		//        result.StatusCode = deserialize.resultCode;
		//        result.Message = deserialize.message;
		//        result.Data = JsonSerializer.Deserialize<MomoCreatePaymentRequest>(contents);
		//        return result;
		//    }
		//    else
		//    {
		//        result.StatusCode = 400;
		//        result.Message = "Fail call to MoMo!";
		//        return result;
		//    }
		//}

		#endregion

		#region Create zalopay - tao rieng zalopay
		public async Task<ResponseObject<Transaction>> CreatePaymentZaloPayAsync(ZaloPayCreatePaymentRequest model)
		{
			var result = new ResponseObject<Transaction>();
			try
			{
				//check validation
				var validateResult = _createZaloPayValidator.Validate(model);
				if ( !validateResult.IsValid )
				{
					var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
					result.StatusCode = 400;
					result.Message = string.Join(" - " , error);
					return result;
				}
				//check orderExist
				var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(model.OrderId.ToString());
				if ( orderExist == null )
				{
					result.StatusCode = 404;
					result.Message = "Order not exist!";
					return result;
				}
				var newTransaction = _mapper.Map<Transaction>(model);
				newTransaction.Id = Guid.NewGuid();//code nay de tao moi id cho transaction - luu y khi sua code
				newTransaction.TransactionDate = DateTime.UtcNow.AddHours(7);
				newTransaction.Type = TransactionType.ZaloPay;
				newTransaction.Status = TransactionStatus.Pending;
				var createResult = await _unitOfWork.TransactionRepository.CreateAsync(newTransaction);
				if ( !createResult )
				{
					result.StatusCode = 500;
					result.Message = "Error!!";
					return result;
				}
				else
				{
					//newTransaction.Order = orderExist;
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Data = newTransaction;
					result.Message = "Create transction success.";
					return result;
				}
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
				return result;
			}
		}


		#endregion

		#region update payment (zalopay)
		public async Task<ResponseObject<Transaction>> UpdatePaymentZaloPayAsync(ZaloPayUpdatePaymentRequest model)
		{
			var result = new ResponseObject<Transaction>();
			//check payment exist
			var zaloPayExist = await _unitOfWork.TransactionRepository.GetByIdAsync(model.Id.ToString());
			if ( zaloPayExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Not found transaction!";
				return result;
			}
			zaloPayExist.Status = model.Status;
			var updateResult = await _unitOfWork.TransactionRepository.UpdateAsync(zaloPayExist);
			if ( updateResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Data = zaloPayExist;
				result.Message = "Update status (" + zaloPayExist.Status + ") of transaction success";
				return result;
			}
			result.StatusCode = 500;
			result.Message = "Update transaction status unsuccess!";
			return result;
		}
		#endregion

		#region create new payment - all - testing
		public async Task<ResponseObject<TransactionResponse>> CreateNewPaymentAsync(CreatePaymentRequest request)
		{
			var result = new ResponseObject<TransactionResponse>();
			try
			{
				//check validation
				var validateResult = _createTransactionValidator.Validate(request);
				if ( !validateResult.IsValid )
				{
					var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
					result.StatusCode = 400;
					result.Message = string.Join(" - " , error);
					return result;
				}
				//check orderExist
				var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId.ToString());
				if ( orderExist == null )
				{
					result.StatusCode = 404;
					result.Message = "Order not exist!";
					return result;
				}
				var newTransaction = _mapper.Map<Transaction>(request);
				newTransaction.Id = Guid.NewGuid();//code nay de tao moi id cho transaction - luu y khi sua code
				newTransaction.TransactionDate = DateTime.UtcNow.AddHours(7).AddHours(7);
				newTransaction.Type = request.TransactionType;
				newTransaction.Status = request.Status == null ? TransactionStatus.Pending : (TransactionStatus)request.Status;

				//newTransaction.Status = (TransactionStatus)request.Status;
				var createResult = await _unitOfWork.TransactionRepository.CreateAsync(newTransaction);
				if ( !createResult )
				{
					result.StatusCode = 500;
					result.Message = "Error!!";
					return result;
				}
				else
				{
					orderExist.Transaction = newTransaction;
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Data = _mapper.Map<TransactionResponse>(newTransaction);
					result.Message = "Create transction success.";
					return result;
				}
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
				return result;
			}
		}
		public async Task<ResponseObject<RefundZaloPayResponse>> RefundTransactionAsync(RefundZaloPayRequest request)
		{
			var result = new ResponseObject<RefundZaloPayResponse>();

			// Check if the transaction exists and if its status is pending
			var transExist = _unitOfWork.TransactionRepository.Get(x => x.Id == request.IdTransaction).FirstOrDefault();
			if ( transExist != null && transExist.Status == TransactionStatus.RefundZaloPayPending )
			{
				// Generate current timestamp
				var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

				// Create data string to sign (MAC)
				var dataToSign = $"{_zaloPayOptions.Value.AppId}|{request.ZpTransId}|{transExist.Amount}|{request.Description}|{timestamp}";
				var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256 , _zaloPayOptions.Value.Key1 , dataToSign);

				// Create dictionary with required parameters for the request
				var param = new Dictionary<string , string>
								{
									{ "app_id", _zaloPayOptions.Value.AppId },
									{ "m_refund_id", Guid.NewGuid().ToString("N") },//N là bỏ dấu -
									{ "zp_trans_id", request.ZpTransId },
									{ "amount", transExist.Amount.ToString() },
									{ "description", request.Description },
									{ "timestamp", timestamp },
									{ "mac", mac }
								};
				// Send the request to ZaloPay API
				using ( var client = new HttpClient() )
				{
					var content = new FormUrlEncodedContent(param);
					var response = await client.PostAsync(_zaloPayOptions.Value.RefundUrl , content);

					if ( response.IsSuccessStatusCode )
					{
						var jsonResponse = await response.Content.ReadAsStringAsync();
						var refundResponse = JsonConvert.DeserializeObject<RefundZaloPayResponse>(jsonResponse);
						//tạm thời chưa đăng ký tài khoản bussiness nên nhận đc response từ zalopay thì sẽ cho là thành công
						if ( refundResponse != null )
						{
							//update transaction status -> paid
							transExist.Status = TransactionStatus.RefundZaloPayDone;
							var updateResult = await _unitOfWork.TransactionRepository.UpdateAsync(transExist);
							if ( updateResult )
							{
								// Change order status to refunded
								var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(request.IdOrder.ToString());
								if ( orderExist != null && orderExist.Status == OrderStatus.Canceled )
								{
									orderExist.Status = OrderStatus.Refund;
									transExist.Order = orderExist;
									transExist.Notice = "Refund zalo pay success";
									orderExist.Transaction = transExist;
									await _unitOfWork.CompleteAsync();
									result.StatusCode = 200;
									result.Message = "Refund successful";
									var customer = await _unitOfWork.UserRepository.GetByIdAsync(orderExist.UserId.ToString());
									if ( customer != null )
									{
										refundResponse.EmailCustomer = customer.Email;
									}
									refundResponse.OrderCode = orderExist.OrderCode.ToString();
									result.Data = refundResponse;
									return result;
								}
								result.StatusCode = 400;
								result.Message = "Refund unsuccessful! Order not in cancel status!";
								return result;

							}
							result.StatusCode = 404;
							result.Message = "Refund failed!Not have transaction.";
							return result;
						}
						else
						{
							result.StatusCode = 400;
							result.Message = refundResponse?.ReturnMessage ?? "ZaloPay returned an error!";
							return result;
						}
					}
					else
					{
						result.StatusCode = 400;
						result.Message = "ZaloPay request failed!";
						return result;
					}
				}
			}
			else
			{
				result.StatusCode = 400;
				result.Message = "Transaction not found or is not pending!";
				return result;
			}
		}




		public async Task<ResponseObject<List<TransactionResponse>>> GetAllAsync()
		{
			var result = new ResponseObject<List<TransactionResponse>>();
			var trans = await _unitOfWork.TransactionRepository.GetAllAsync();
			if ( trans != null )
			{
				result.StatusCode = 200;
				result.Message = "List transactions.";
				result.Data = _mapper.Map<List<TransactionResponse>>(trans);
				return result;
			}
			result.StatusCode = 400;
			result.Message = "Not have transaction!";
			return result;
		}

		#endregion

	}
}
