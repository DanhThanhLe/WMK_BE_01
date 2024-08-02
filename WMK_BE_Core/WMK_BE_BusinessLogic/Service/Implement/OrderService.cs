using AutoMapper;
using Azure.Core;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderModel;
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
		public async Task<ResponseObject<List<OrderResponse>>> GetAllOrders()
		{
			var result = new ResponseObject<List<OrderResponse>>();
			var orders = await _unitOfWork.OrderRepository.GetAllAsync();
			if ( orders != null && orders.Count > 0 )
			{
				result.StatusCode = 200;
				result.Message = "Order List";
				var orderR = _mapper.Map<List<OrderResponse>>(orders);
				foreach ( var item in orderR )
				{
					var user = await _unitOfWork.UserRepository.GetByIdAsync(item.UserId.ToString());
					if ( user != null )
					{
						item.UserId = user.FirstName + " " + user.LastName;
					}
				}
				result.Data = orderR;
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Don't have Order!";
				return result;
			}
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
					result.Message = "Empty";
					return result;
				}

				//tren moblie chi can get trans moi nhat
				foreach ( var item in foundList )
				{
					if ( item.Transactions != null && item.Transactions.Count > 1 )
						item.Transactions = item.Transactions.OrderByDescending(x => x.TransactionDate).Take(1).ToList();
				}
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

		#endregion

		#region change status
		public async Task<ResponseObject<OrderResponse>> ChangeStatusOrderAsync(Guid id , ChangeStatusOrderRequest model)
		{
			var result = new ResponseObject<OrderResponse>();

			var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(id.ToString());
			if ( orderExist != null )
			{
				orderExist.Status = model.Status;
				var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(orderExist);
				if ( updateResult )
				{

					//await _unitOfWork.CompleteAsync();
					////change status transaction
					//foreach (var zaloPay in orderExist.Transactions)
					//{
					//    zaloPay.Status = TransactionStatus.PAID;
					//    zaloPay.TransactionDate = DateTime.Now;
					//}
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Change order status into " + orderExist.Status + " success.";
					//result.Data = _mapper.Map<OrderResponse>(orderExist);
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
			//Tinh so luong trong recipeList

			//tao order code
			Random random = new Random();
			int minValue = 10000000;
			int maxValue = 99999999;
			Order checkOrderCode = new Order();
			int randomOrderCode = 0;
			//do//check toi khi nao order code ko trung thi thoi
			//{
			randomOrderCode = random.Next(minValue , maxValue + 1);
			//checkOrderCode = _unitOfWork.OrderRepository.Get(x => x.OrderCode == randomOrderCode).FirstOrDefault();
			//} while (checkOrderCode.Id.ToString() != null);
			//tao order code
			_unitOfWork.OrderRepository.DetachEntity(checkOrderCode);

			//chua tinh duoc total price (dự kiến tính bằng cách nhân quantity với price của từng sản phẩm trong listRecipe nếu là custom hoặc là lấy giá của weeklyPlan nếu là formal
			var newOrder = _mapper.Map<Order>(model);
			newOrder.OrderCode = randomOrderCode;
			//newOrder.TotalPrice = model.TotalPrice * 1000;
			newOrder.OrderDate = DateTime.Now;
			switch ( DateTime.Now.DayOfWeek )
			{
				case DayOfWeek.Monday:
					newOrder.ShipDate = DateTime.Now.AddDays(6);
					break;
				case DayOfWeek.Tuesday:
					newOrder.ShipDate = DateTime.Now.AddDays(5);
					break;
				case DayOfWeek.Wednesday:
					newOrder.ShipDate = DateTime.Now.AddDays(4);
					break;
				case DayOfWeek.Thursday:
					newOrder.ShipDate = DateTime.Now.AddDays(3);
					break;
				case DayOfWeek.Friday:
					newOrder.ShipDate = DateTime.Now.AddDays(2);
					break;
				case DayOfWeek.Saturday:
					newOrder.ShipDate = DateTime.Now.AddDays(8);
					break;
				case DayOfWeek.Sunday:
					newOrder.ShipDate = DateTime.Now.AddDays(7);
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
							result.Message = "OK. Create order success";
							result.Data = newOrder.Id;
							return result;
						}
						if ( createTransactionResult.StatusCode != 200 )//code cu la (createTransaction != null)
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
			result.StatusCode = 500;
			result.Message = "OK. Create order not success";
			return result;
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
	}
}
