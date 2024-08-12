using AutoMapper;
using Azure.Core;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class OrderService : IOrderService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ITransactionService _transactionService;
		#region Validator
		private readonly CreateOrderModelValidator _createOrderValidator;
		private readonly UpdateOrderModelValidator _updateOrderValidator;
		private readonly UpdateOrderByUserModelValidator _updateOrderByUserValidator;
		private readonly IOrderDetailService _orderDetailService;

		#endregion
		public OrderService(IUnitOfWork unitOfWork , IMapper mapper , IOrderDetailService orderDetailService , ITransactionService transactionService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_transactionService = transactionService;
			_createOrderValidator = new CreateOrderModelValidator();
			_updateOrderValidator = new UpdateOrderModelValidator();
			_updateOrderByUserValidator = new UpdateOrderByUserModelValidator();
			_orderDetailService = orderDetailService;
		}
		#region get all
		public async Task<ResponseObject<List<OrderResponse>>> GetAllOrdersAsync(GetAllOrdersRequest? model)
		{
			var result = new ResponseObject<List<OrderResponse>>();

			var orders = new List<Order>();
			var ordersResponse = new List<OrderResponse>();
			if ( model != null && (!model.ReceiveName.IsNullOrEmpty() || !model.OrderCode.IsNullOrEmpty()) )
			{
				if ( !model.ReceiveName.IsNullOrEmpty() )
				{
					var ordersByReciveName = await GetOrdersByReciveName(model.ReceiveName);
					if ( ordersByReciveName != null && ordersByReciveName.Data != null )
					{
						ordersResponse.AddRange(ordersByReciveName.Data);
					}
				}
				if ( !model.OrderCode.IsNullOrEmpty() )
				{
					var ordersByOrderCode = await GetOrdersByOrderCode(model.OrderCode);
					if ( ordersByOrderCode != null && ordersByOrderCode.Data != null )
					{
						ordersResponse.AddRange(ordersByOrderCode.Data);
					}
				}
				// Loại bỏ các phần tử trùng lặp dựa trên Id
				ordersResponse = ordersResponse
					.GroupBy(c => c.Id)
					.Select(g => g.First())
					.ToList();
			}
			else
			{
				orders = await _unitOfWork.OrderRepository.GetAllAsync();
				ordersResponse = _mapper.Map<List<OrderResponse>>(orders);
			}
			if ( ordersResponse != null && ordersResponse.Any() )
			{

				foreach ( var item in ordersResponse )
				{
					var user = await _unitOfWork.UserRepository.GetByIdAsync(item.UserId.ToString());
					if ( user != null )
					{
						item.UserId = user.FirstName + " " + user.LastName;
					}
				}
				result.StatusCode = 200;
				result.Message = "Get order list success (" + ordersResponse.Count() + ")";
				result.Data = ordersResponse.OrderBy(o => o.OrderDate).ToList();
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Don't have Order!";
				result.Data = [];
				return result;
			}
		}

		public async Task<ResponseObject<List<OrderResponse>>> GetOrdersByReciveName(string reciveName)
		{
			var result = new ResponseObject<List<OrderResponse>>();

			var orders = await _unitOfWork.OrderRepository.GetAllAsync();

			orders = orders.Where(o => o.ReceiveName.ToLower().RemoveDiacritics().Contains(reciveName.ToLower().RemoveDiacritics())).ToList();
			if ( orders != null && orders.Any() )
			{
				result.StatusCode = 200;
				result.Message = "Order list found by recive name success";
				result.Data = _mapper.Map<List<OrderResponse>>(orders);
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Order list not found by recive name: " + reciveName;
			return result;

		}
		public async Task<ResponseObject<List<OrderResponse>>> GetOrdersByOrderCode(string orderCode)
		{
			var result = new ResponseObject<List<OrderResponse>>();

			var orders = await _unitOfWork.OrderRepository.GetAllAsync();

			orders = orders.Where(o => o.OrderCode.ToString().ToLower().RemoveDiacritics().Contains(orderCode.ToLower().RemoveDiacritics())).ToList();
			if ( orders != null && orders.Any() )
			{
				result.StatusCode = 200;
				result.Message = "Order list found by orderCode success";
				result.Data = _mapper.Map<List<OrderResponse>>(orders);
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Order list not found by orderCode: " + orderCode;
			return result;

		}

		#endregion

		#region Get orders by user id
		public async Task<ResponseObject<List<OrderResponse>>> GetOrdersByUserId(Guid userId)
		{
			var result = new ResponseObject<List<OrderResponse>>();
			try
			{
				//tim thong tin cua order lien quan toi id nguoi  dung
				var foundList = _unitOfWork.OrderRepository.Get(x => x.UserId == userId).ToList();
				if ( foundList.Count() == 0 )
				{
					result.StatusCode = 500;
					result.Message = "Don't have order by this user!";
					return result;
				}

				//tren moblie chi can get trans moi nhat
				//foreach ( var item in foundList )
				//{
				//	if ( item.Transaction != null && item.Transaction.Count > 1 )
				//		item.Transaction = item.Transaction.OrderByDescending(x => x.TransactionDate).Take(1).ToList();
				//}
				result.StatusCode = 200;
				result.Message = "Ok, list order " + foundList.Count();
				var returnList = _mapper.Map<List<OrderResponse>>(foundList);
				result.Data = returnList;
				return result;
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
				return result;
			}
		}
		#endregion

		#region get order by id
		public async Task<ResponseObject<OrderResponse?>> GetOrderByIdAsync(Guid id)
		{
			var result = new ResponseObject<OrderResponse?>();
			var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(id.ToString());
			if ( orderExist != null )
			{
				result.StatusCode = 200;
				result.Message = "Order:";
				result.Data = _mapper.Map<OrderResponse>(orderExist);
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Order not found!";
				return result;
			}
		}

		#endregion

		#region create
		public async Task<ResponseObject<Guid>> CreateOrderAsync(CreateOrderRequest model)
		{
			var result = new ResponseObject<Guid>();
			var validationResult = _createOrderValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			//check userId exists
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(model.UserId);
			if ( userExist == null )
			{
				result.StatusCode = 404;
				result.Message = "User not found!";
				return result;
			}
			//check recipeList
			if ( model.RecipeList == null )
			{
				result.StatusCode = 404;
				result.Message = "Recipe list null! Please input recipe!!";
				return result;
			}
			int quantity = 0;
			foreach ( var item in model.RecipeList )//tính số lượng phần ăn (dưới 5 hoặc trên 200 ko cho đặt)
			{
				quantity += item.Quantity;
			}
			if ( quantity < 5 || quantity > 200 )//ko đáp ứng quy định phần ăn quy định (5-200)
			{
				result.StatusCode = 500;
				result.Message = "Vui lòng đặt ít nhất 5 món ăn hoặc 5 phần ăn. Nhiều nhất 200 phần/món";
				return result;
			}
			// Tạo mã đơn hàng duy nhất
			var randomOrderCode = await GenerateUniqueOrderCodeAsync();
			var newOrder = _mapper.Map<Order>(model);
			newOrder.OrderCode = randomOrderCode;
			newOrder.OrderDate = DateTime.UtcNow;
			switch ( DateTime.UtcNow.DayOfWeek )
			{
				case DayOfWeek.Monday:
					newOrder.ShipDate = DateTime.UtcNow.AddDays(6);
					break;
				case DayOfWeek.Tuesday:
					newOrder.ShipDate = DateTime.UtcNow.AddDays(5);
					break;
				case DayOfWeek.Wednesday:
					newOrder.ShipDate = DateTime.UtcNow.AddDays(4);
					break;
				case DayOfWeek.Thursday:
					newOrder.ShipDate = DateTime.UtcNow.AddDays(3);
					break;
				case DayOfWeek.Friday:
					newOrder.ShipDate = DateTime.UtcNow.AddDays(2);
					break;
				case DayOfWeek.Saturday:
					newOrder.ShipDate = DateTime.UtcNow.AddDays(8);
					break;
				case DayOfWeek.Sunday:
					newOrder.ShipDate = DateTime.UtcNow.AddDays(7);
					break;
			}
			newOrder.Status = OrderStatus.Processing;
			var createResult = await _unitOfWork.OrderRepository.CreateAsync(newOrder);
			if ( createResult )//bat dau add cac recipeId thanh cac OrderDetail thong qua RecipeList
			{
				await _unitOfWork.CompleteAsync();
				if ( model.RecipeList.Any() )
				{
					var createOrderDetailResult = await _orderDetailService.CreateOrderDetailAsync(newOrder.Id , model.RecipeList);
					if ( createOrderDetailResult.StatusCode == 200 && createOrderDetailResult.Data != null )
					{
						await _unitOfWork.CompleteAsync();
						//create transaction
						//phan biet loai transaction
						var newPaymentRequest = new CreatePaymentRequest();
						newPaymentRequest.OrderId = newOrder.Id;
						newPaymentRequest.Amount = newOrder.TotalPrice;
						newPaymentRequest.TransactionType = model.TransactionType;
						var createTransactionResult = await _transactionService.CreateNewPaymentAsync(newPaymentRequest);
						if ( createTransactionResult != null && createTransactionResult.StatusCode == 200 && createTransactionResult.Data != null )
						{
							//newOrder.Transactions.Add(createTransaction.Data);
							await _unitOfWork.CompleteAsync();
							result.StatusCode = 200;
							result.Message = "Create order success";
							result.Data = newOrder.Id;
							return result;
						}
						if ( createTransactionResult != null && createTransactionResult.StatusCode != 200 )//code cu la (createTransaction != null)
						{
							// Delete the order if transaction creation fails
							await _unitOfWork.OrderRepository.DeleteAsync(newOrder.Id.ToString());
							await _unitOfWork.CompleteAsync();
							result.StatusCode = createTransactionResult.StatusCode;
							result.Message = createTransactionResult.Message;
							return result;
						}
					}
					//luc nay la vi li do gi do ko tao duoc thong tin detail (customPlan cho order) -> xoa order. thong bao ko tao thanh cong
					await _unitOfWork.OrderRepository.DeleteAsync(newOrder.Id.ToString());//xoa thong tin cho Order
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 500;
					result.Message = "Create order not success!";
					return result;
				}
			}
			await _unitOfWork.OrderRepository.DeleteAsync(newOrder.Id.ToString());
			await _unitOfWork.CompleteAsync();
			result.StatusCode = 500;
			result.Message = "Create order not success!";
			return result;
		}
		// Hàm để tạo mã đơn hàng ngẫu nhiên và đảm bảo tính duy nhất
		private async Task<int> GenerateUniqueOrderCodeAsync()
		{
			Random random = new Random();
			int minValue = 10000000;
			int maxValue = 99999999;
			int randomOrderCode;
			bool isUnique;

			do
			{
				randomOrderCode = random.Next(minValue , maxValue + 1);
				// Kiểm tra xem mã đơn hàng đã tồn tại trong cơ sở dữ liệu chưa
				isUnique = !await _unitOfWork.OrderRepository.Get(x => x.OrderCode == randomOrderCode).AnyAsync();
			}
			while ( !isUnique ); // Lặp lại cho đến khi tìm thấy mã đơn hàng duy nhất

			return randomOrderCode;
		}
		#endregion

		#region update order
		public async Task<ResponseObject<OrderResponse>> UpdateOrderAsync(string id , UpdateOrderRequest model)
		{
			var result = new ResponseObject<OrderResponse>();
			var validationResult = _updateOrderValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			//find order
			var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(id.ToString());
			if ( orderExist != null )
			{
				var updateOrder = _mapper.Map<Order>(model);
				updateOrder.Id = orderExist.Id;
				var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(updateOrder);
				if ( updateResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Update successfully.";
					result.Data = _mapper.Map<OrderResponse>(updateOrder);
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Update faild!";
					return result;
				}
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Order not found!";
				return result;
			}
		}
		public async Task<ResponseObject<OrderResponse>> ChangeOrderGroupAsync(Guid idOrder , Guid idOrderGroup)
		{
			var result = new ResponseObject<OrderResponse>();

			//check order group exist
			var orderGroupExist = await _unitOfWork.OrderGroupRepository.GetByIdAsync(idOrderGroup.ToString());

			if ( orderGroupExist != null )
			{
				var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(idOrder.ToString());
				if ( orderExist != null )
				{
					orderExist.OrderGroupId = idOrderGroup;
					orderExist.OrderGroup = orderGroupExist;
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Change order group success";
					return result;
				}
			}

			result.StatusCode = 404;
			result.Message = "OrderGroup not exist!";
			return result;
		}
		public async Task<ResponseObject<OrderResponse>> ChangeStatusOrderAsync(Guid id , ChangeStatusOrderRequest model)
		{
			var result = new ResponseObject<OrderResponse>();

			var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(id.ToString());
			if ( orderExist != null )
			{
				////nếu change qua refun thì kiểm tra xem trạng thái transaction đã paid chưa
				//if ( model.Status == OrderStatus.Refund )
				//{
				//	//chưa paid thì không được đổi qua refun
				//	foreach ( var transaction in orderExist.Transactions )
				//	{
				//		if ( transaction.Status != TransactionStatus.PAID )
				//		{
				//			result.StatusCode = 400;
				//			result.Message = "Payment not successful can't refun!";
				//			return result;
				//		}
				//	}
				//}
				//if ( orderExist.Status == OrderStatus.Canceled && model.Status == OrderStatus.Refund )
				//{
				//	//chỉ được đổi qua refun nếu transaction paid
				//	foreach ( var transaction in orderExist.Transactions )
				//	{
				//		if ( transaction.Status != TransactionStatus.PAID )
				//		{
				//			result.StatusCode = 400;
				//			result.Message = "Order is cancel can't change status!";
				//			return result;
				//		}
				//	}
				//}
				//map
				orderExist.Status = model.Status;
				//if ( orderExist.Status == OrderStatus.Shipping )
				//{
				//	//nếu là cod thì đổi lại transaction paid
				//	//cập nhập lại transaction đã thanh toán rồi
				//	foreach ( var transaction in orderExist.Transactions )
				//	{

				//		if ( transaction.Type != TransactionType.COD )
				//		{
				//			if ( transaction.Status != TransactionStatus.Cancel )
				//			{
				//				transaction.Status = TransactionStatus.PAID;
				//			}
				//		}
				//	}
				//}
				//nếu order thành công -> status chuyển sang shipped (do shipper nhấn) thì sẽ tăng pop của recipe lên
				if ( model.Status == OrderStatus.Shipped )
				{
					//tăng  pop trong từng recipe
					foreach ( var orderDetail in orderExist.OrderDetails )
					{
						var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(orderDetail.RecipeId.ToString());
						if ( recipeExist != null )
						{
							recipeExist.Popularity++;
						}
					}
				}
				var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(orderExist);
				if ( updateResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Change order status into " + orderExist.Status + " success.";
					result.Data = _mapper.Map<OrderResponse>(orderExist);
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Fail to update order!";
					return result;
				}
			}
			result.StatusCode = 404;
			result.Message = "Order not exist!";
			return result;

		}
		#endregion

		#region update order by user ...?
		public async Task<ResponseObject<OrderResponse>> UpdateOrderByUserAsync(UpdateOrderByUserRequest model)
		{
			var result = new ResponseObject<OrderResponse>();
			var validationResult = _updateOrderByUserValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			//find order
			var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(model.Id.ToString());
			if ( orderExist != null )
			{
				var updateOrder = _mapper.Map<Order>(model);
				var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(updateOrder);
				if ( updateResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Update successfully.";
					result.Data = _mapper.Map<OrderResponse>(updateOrder);
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Update faild!";
					return result;
				}
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Order not found!";
				return result;
			}
		}

		#endregion

		#region delete
		public async Task<ResponseObject<OrderResponse>> DeleteOrderAsync(Guid id)
		{
			var result = new ResponseObject<OrderResponse>();

			var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(id.ToString());
			if ( orderExist != null )
			{
				var userExist = await _unitOfWork.OrderRepository.GetUserExistInOrderAsync(orderExist.Id , orderExist.UserId);
				if ( userExist )
				{
					//change order status
					orderExist.Status = OrderStatus.Canceled;
					var orderUpdate = await _unitOfWork.OrderRepository.UpdateAsync(orderExist);
					if ( orderUpdate )
					{
						await _unitOfWork.CompleteAsync();
						result.StatusCode = 200;
						result.Message = "Change status order (" + orderExist.Id + ") into canceled success.";
						return result;
					}
					else
					{
						result.StatusCode = 500;
						result.Message = "Fail to change status went delete order!";
						return result;
					}
				}
				else
				{
					var orderDelete = await _unitOfWork.OrderRepository.DeleteAsync(orderExist.Id.ToString());
					if ( orderDelete )
					{
						await _unitOfWork.CompleteAsync();
						result.StatusCode = 200;
						result.Message = "Delete order success.";
						result.Data = _mapper.Map<OrderResponse>(orderExist);
						return result;
					}
					else
					{
						result.StatusCode = 500;
						result.Message = "Delete order unsucces!";
						return result;
					}
				}
			}
			result.StatusCode = 404;
			result.Message = "Order not exist!";
			return result;
		}
		public async Task<ResponseObject<OrderResponse>> RemoveOrderFormOrderGroupAsync(Guid idOrder)
		{
			var result = new ResponseObject<OrderResponse>();

			var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(idOrder.ToString());
			if ( orderExist != null )
			{
				orderExist.OrderGroupId = null;
				orderExist.OrderGroup = null;
				var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(orderExist);
				if ( updateResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Remove Order from orde rgoup success";
					return result;
				}
				result.StatusCode = 500;
				result.Message = "Remove Order from orde rgoup unsuccess!";
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Order not exist!";
			return result;

		}

		public async Task<ResponseObject<List<OrderResponse>>> RemoveAllOrdersFromOrderGroupsAsync()
		{
			var result = new ResponseObject<List<OrderResponse>>();
			var orderResponses = new List<OrderResponse>();

			try
			{
				// Lấy tất cả các order có OrderGroupId không null
				var ordersWithOrderGroups = await _unitOfWork.OrderRepository.GetAllAsync();
				ordersWithOrderGroups = ordersWithOrderGroups.Where(o => o.OrderGroupId != null).ToList();

				if ( ordersWithOrderGroups.Any() )
				{
					foreach ( var order in ordersWithOrderGroups )
					{
						order.OrderGroupId = null;
						order.OrderGroup = null;
						var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(order);

						if ( updateResult )
						{
							orderResponses.Add(_mapper.Map<OrderResponse>(order));
						}
						else
						{
							result.StatusCode = 500;
							result.Message = "An error occurred while updating orders.";
							return result;
						}
					}

					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Successfully removed all orders from their order groups.";
					result.Data = orderResponses;
					return result;
				}
				else
				{
					result.StatusCode = 404;
					result.Message = "No orders found with an order group.";
					return result;
				}
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = $"Error when processing: {ex.Message}";
				return result;
			}
		}

		#endregion


	}
}
