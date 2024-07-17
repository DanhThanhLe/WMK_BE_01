using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel;
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
		#region Validator
		private readonly CreateOrderModelValidator _createOrderValidator;
		private readonly UpdateOrderModelValidator _updateOrderValidator;

		#endregion
		public OrderService(IUnitOfWork unitOfWork , IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;

			_createOrderValidator = new CreateOrderModelValidator();
			_updateOrderValidator = new UpdateOrderModelValidator();
		}
		public async Task<ResponseObject<List<OrderResponse>>> GetAllOrders()
		{
			var result = new ResponseObject<List<OrderResponse>>();
			var orders = await _unitOfWork.OrderRepository.GetAllAsync();
			if ( orders != null && orders.Count > 0 )
			{
				result.StatusCode = 200;
				result.Message = "Order List";
				result.Data = _mapper.Map<List<OrderResponse>>(orders);
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Don't have Order!";
				return result;
			}
		}

		public async Task<ResponseObject<OrderResponse?>> GetOrderByIdAsync(IdOrderRequest model)
		{
			var result = new ResponseObject<OrderResponse?>();
			var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(model.Id.ToString());
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

		public async Task<ResponseObject<OrderResponse>> CreateOrderAsync(CreateOrderRequest model)
		{
			var result = new ResponseObject<OrderResponse>();
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
			//chua tinh duoc total price (dự kiến tính bằng cách nhân quantity với price của từng sản phẩm trong listRecipe nếu là custom hoặc là lấy giá của weeklyPlan nếu là formal
			var newOrder = _mapper.Map<Order>(model);
			newOrder.User = userExist;
			//newOrder.TotalPrice = model.TotalPrice * 1000;
			newOrder.OrderDate = DateTime.Now;
			newOrder.Status = OrderStatus.Processing;
			newOrder.User = userExist;

			var createResult = await _unitOfWork.OrderRepository.CreateAsync(newOrder);
			if ( createResult )
			{
				//check if weekly plan not null then asign with order 
				Guid weeklyPlanId;
				if ( model.StanderdWeeklyPlanId != null )
				{
					if ( Guid.TryParse(model.StanderdWeeklyPlanId , out weeklyPlanId) )
					{
						var weeklyPlanExist = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(weeklyPlanId.ToString());
						if ( weeklyPlanExist == null )
						{
							result.StatusCode = 404;
							result.Message = "WeeklyPlan not exist!";
							return result;
						}
						newOrder.StanderdWeeklyPlanId = weeklyPlanId;
						newOrder.WeeklyPlan = weeklyPlanExist;
						double totalPrice = 0;//phan nay tinh total price cho order formal (la dat han theo weeklyPlan co san)
						foreach(var item in weeklyPlanExist.RecipePLans)
						{
							totalPrice += item.Recipe.Price * item.Quantity;
						}
						newOrder.TotalPrice = totalPrice;
						newOrder.ShipDate = DateTime.Now.AddDays(7);
						await _unitOfWork.CompleteAsync(); //save order to DB
					}
					else
					{
						result.StatusCode = 401;
						result.Message = "WeeklyPlanId wrong fortmat GUID!";
						return result;
					}
				}
				else if ( model.RecipeList != null && model.RecipeList.Any() )
				{
					await _unitOfWork.CompleteAsync(); //save order to DB
													   //create order by custom plan
					foreach ( var customPlanRequest in model.RecipeList )
					{
						var customPlan = new CustomPlan
						{
							OrderId = newOrder.Id ,
							RecipeId = customPlanRequest.RecipeId ,
							StandardWeeklyPlanId = customPlanRequest.StandardWeeklyPlanId ,
							Price = customPlanRequest.Price ,
							Order = newOrder
						};
						var createCustomPlanResult = await _unitOfWork.CustomPlanRepository.CreateAsync(customPlan);
						if ( !createCustomPlanResult )
						{
							result.StatusCode = 500;
							result.Message = "Create custom plan unsuccessfully!";
							return result;
						}
					}
					await _unitOfWork.CompleteAsync();//save custom plan to DB
				}
				else
				{
					result.StatusCode = 403;
					result.Message = "Create order unsuccessfully!Don't have weekly pLan or custom plan!";
					return result;
				}
				userExist.Orders.Add(newOrder);
				await _unitOfWork.CompleteAsync(); //save order to DB
				result.StatusCode = 200;
				result.Message = "Created order of user(" + userExist.FirstName + userExist.LastName + ") successfully.";
				result.Data = _mapper.Map<OrderResponse>(newOrder);
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Create order unsuccessfully!";
				return result;
			}
		}

		public async Task<ResponseObject<OrderResponse>> UpdateOrderAsync(UpdateOrderRequest model)
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
			//check order exists
			var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(model.Id);
			if ( orderExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Order not found!";
				return result;
			}
			if ( !string.IsNullOrEmpty(model.StanderdWeeklyPlanId) )
			{
				Guid weeklyPlanId;
				if ( Guid.TryParse(model.StanderdWeeklyPlanId , out weeklyPlanId) )
				{
					var weeklyPlanExist = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(weeklyPlanId.ToString());
					if ( weeklyPlanExist == null )
					{
						result.StatusCode = 404;
						result.Message = "WeeklyPlan not exist!";
						return result;
					}
					orderExist.StanderdWeeklyPlanId = weeklyPlanId;
					orderExist.WeeklyPlan = weeklyPlanExist;
				}
			}
			if ( !string.IsNullOrEmpty(model.Note) )
				orderExist.Note = model.Note;
			if ( !string.IsNullOrEmpty(model.Address) )
				orderExist.Address = model.Address;
			if ( model.TotalPrice > 0 )
				orderExist.TotalPrice = (double)(model.TotalPrice * 1000);
			if ( model.ShipDate.HasValue )
				orderExist.ShipDate = model.ShipDate.Value;

			var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(orderExist);
			if ( updateResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Updated order of user(" + orderExist.User.FirstName + orderExist.User.LastName + ") successfully.";
				result.Data = _mapper.Map<OrderResponse>(orderExist);
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Update order unsuccessfully!";
				return result;
			}
		}

		public async Task<ResponseObject<OrderResponse>> DeleteOrderAsync(IdOrderRequest model)
		{
			var result = new ResponseObject<OrderResponse>();

			var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(model.Id.ToString());
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

		public async Task<ResponseObject<OrderResponse>> ChangeStatusOrderAsync(ChangeStatusOrderRequest model)
		{
			var result = new ResponseObject<OrderResponse>();

			var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(model.Id.ToString());
			if ( orderExist != null )
			{
				orderExist.Status = model.Status;
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
	}
}
