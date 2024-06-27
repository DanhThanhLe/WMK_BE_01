using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderGroupModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderGroupModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class OrderGroupService : IOrderGroupService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public OrderGroupService(IUnitOfWork unitOfWork , IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}


		public async Task<ResponseObject<List<OrderGroupsResponse>>> GetAllAsync()
		{
			var result = new ResponseObject<List<OrderGroupsResponse>>();

			var orderGroupList = await _unitOfWork.OrderGroupRepository.GetAllAsync();
			if ( orderGroupList != null && orderGroupList.Count > 0 )
			{
				result.StatusCode = 200;
				result.Message = "OrderGroup list";
				result.Data = _mapper.Map<List<OrderGroupsResponse>>(orderGroupList);
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Dont have order group!";
				return result;
			}
		}

		public async Task<ResponseObject<OrderGroupsResponse?>> GetOrderGroupByIdAsync(Guid orderGroupId)
		{
			var result = new ResponseObject<OrderGroupsResponse?>();
			var orderGroupExist = await _unitOfWork.OrderGroupRepository.GetByIdAsync(orderGroupId.ToString());
			if ( orderGroupExist != null )
			{
				result.StatusCode = 200;
				result.Message = "OrderGroup";
				result.Data = _mapper.Map<OrderGroupsResponse>(orderGroupExist);
				return result;
			}
			else
			{
				result.StatusCode = 200;
				result.Message = "OrderGroup not exist!";
				return result;
			}
		}

		public async Task<ResponseObject<OrderGroupsResponse>> CreateOrderGroupAsync(CreateOrderGroupRequest model)
		{
			var result = new ResponseObject<OrderGroupsResponse>();

			//check shipper exist
			var shipperExist = await _unitOfWork.UserRepository.GetByIdAsync(model.ShipperId.ToString());
			if ( shipperExist != null && shipperExist.Role != Role.Shipper )
			{
				result.StatusCode = 403;
				result.Message = "Not a Shipper!";
				return result;
			}
			else if ( shipperExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Shipper not exist!";
				return result;
			}

			var staffExist = await _unitOfWork.UserRepository.GetByIdAsync(model.AsignBy.ToString());
			if ( staffExist != null && staffExist.Role != Role.Staff )
			{
				result.StatusCode = 403;
				result.Message = "Not a staff!";
				return result;
			}
			else if ( staffExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Staff not exist!";
				return result;
			}

			var orderGroupModel = _mapper.Map<OrderGroup>(model);
			orderGroupModel.AsignAt = DateTime.Now;
			orderGroupModel.Status = BaseStatus.Available;
			orderGroupModel.User = shipperExist;
			var createResult = await _unitOfWork.OrderGroupRepository.CreateAsync(orderGroupModel);
			if ( createResult )
			{
				await _unitOfWork.CompleteAsync();
				shipperExist.OrderGroup = orderGroupModel;
				result.StatusCode = 200;
				result.Message = "Create order group successfully";
				result.Data = _mapper.Map<OrderGroupsResponse>(orderGroupModel);
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Create order group unsuccessfully";
				return result;
			}
		}

		public async Task<ResponseObject<OrderGroupsResponse>> UpdateOrderGroupAsync(UpdateOrderGroupRequest model)
		{
			var result = new ResponseObject<OrderGroupsResponse>();
			//check new shipper
			var shipperExist = await _unitOfWork.UserRepository.GetByIdAsync(model.ShipperId.ToString());
			if ( shipperExist != null && shipperExist.Role != Role.Shipper )
			{
				result.StatusCode = 403;
				result.Message = "Not a Shipper!";
				return result;
			}
			else if ( shipperExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Shipper not exist!";
				return result;
			}
			var orderGroupExist = await _unitOfWork.OrderGroupRepository.GetByIdAsync(model.Id.ToString());
			if ( orderGroupExist != null )
			{
				var orderGroup = _mapper.Map(orderGroupExist , model);
				var updateResult = await _unitOfWork.OrderGroupRepository.UpdateAsync(orderGroupExist);
				if ( updateResult )
				{
					orderGroupExist.User = shipperExist;
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Update order group successfully.";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Update order group unsuccessfully!";
					return result;
				}
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Order group not exist!";
				return result;
			}

		}

		public async Task<ResponseObject<OrderGroupsResponse>> DeleteOrderGroupAsync(IdOrderGroupRequest model)
		{
			var result = new ResponseObject<OrderGroupsResponse>();
			var orderGroupExist = await _unitOfWork.OrderGroupRepository.GetByIdAsync(model.Id.ToString());
			if ( orderGroupExist != null )
			{
				var orderGroupExistOrder = _unitOfWork.OrderGroupRepository.OrderGroupExistOrder(orderGroupExist);
				if ( orderGroupExistOrder )
				{
					//change status
					orderGroupExist.Status = BaseStatus.UnAvailable;
					var updateResult = await _unitOfWork.OrderGroupRepository.UpdateAsync(orderGroupExist);
					if ( updateResult )
					{
						await _unitOfWork.CompleteAsync();
						result.StatusCode = 200;
						result.Message = "Trust change order group status.";
						return result;
					}
					else
					{
						result.StatusCode = 500;
						result.Message = "Change order group status unsucess!";
						return result;
					}
				}
				else
				{
					var deleteResult = await _unitOfWork.OrderGroupRepository.DeleteAsync(model.Id.ToString());
					if ( deleteResult )
					{
						await _unitOfWork.CompleteAsync();
						result.StatusCode = 200;
						result.Message = "Delete order group successfully.";
						return result;
					}
					else
					{
						result.StatusCode = 500;
						result.Message = "Delete order group unsuccessfully!";
						return result;
					}
				}
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "OrderGroup not exist!";
				return result;
			}
		}

		public async Task<ResponseObject<OrderGroupsResponse>> ChangeStatusOrderGroupAsync(IdOrderGroupRequest model)
		{
			var result = new ResponseObject<OrderGroupsResponse>();
			var orderGroupExist = await _unitOfWork.OrderGroupRepository.GetByIdAsync(model.Id.ToString());
			if ( orderGroupExist != null && orderGroupExist.Status == BaseStatus.Available )
			{
				switch ( orderGroupExist.Status )
				{
					case BaseStatus.Available:
						orderGroupExist.Status = BaseStatus.UnAvailable;
						break;
					case BaseStatus.UnAvailable:
						orderGroupExist.Status = BaseStatus.Available;
						break;
				}
				var updateResult = await _unitOfWork.OrderGroupRepository.UpdateAsync(orderGroupExist);
				if ( updateResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Change status order group (" + orderGroupExist.Status + ") successfully.";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Change status order group unsuccessfully!";
					return result;
				}
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Order group not exist!";
				return result;
			}
		}

		#region Cluster
		public async Task<ResponseObject<List<OrderGroupsResponse>>> OrderGroupClusterAsync(ClusterOrderGroupRequest model)
		{
			var result = new ResponseObject<List<OrderGroupsResponse>>();
			var list = new List<List<string>>();
			var orderGroups = await _unitOfWork.OrderGroupRepository.GetAllAsync();
			var orders = await _unitOfWork.OrderRepository.GetAllAsync();
			if ( orders != null && orderGroups != null)
			{
				// Group addresses by nearest cluster
				var orderProcess = orders.Where(o => o.Status == OrderStatus.Processing).ToList();
				foreach(var order in orderProcess)
				{
					double[] orderCoordinates = order.Coordinates;
					OrderGroup nearestOrderGroup = new OrderGroup();
					double nearestDistance = double.MaxValue;

					foreach (var orderGroup in orderGroups)
					{
						double[] orderGroupCoordinates = orderGroup.Coordinates;
						double distance = CalculateDistance(orderCoordinates , orderGroupCoordinates);
						if ( distance < nearestDistance && distance <= model.radius )
						{
							nearestDistance = distance;
							nearestOrderGroup = orderGroup;
						}
					}
					if ( nearestOrderGroup != null )
					{
						if ( nearestOrderGroup.Orders == null )
						{
							nearestOrderGroup.Orders = new List<Order>();
						}
						nearestOrderGroup.Orders.Add(order);
						order.OrderGroup = nearestOrderGroup;
						order.OrderGroupId = nearestOrderGroup.Id;
					}
				}
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Cluster successfully.";
				result.Data = _mapper.Map<List<OrderGroupsResponse>>(orderGroups);
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Order or order group not have!";
				return result;
			}
		}

		private static double CalculateDistance(double[] point1 , double[] point2)
		{
			var d1 = point1[0] * (Math.PI / 180.0);
			var num1 = point1[1] * (Math.PI / 180.0);
			var d2 = point2[0] * (Math.PI / 180.0);
			var num2 = point2[1] * (Math.PI / 180.0) - num1;
			var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0) , 2.0) +
					 Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0) , 2.0);

			return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3) , Math.Sqrt(1.0 - d3))) / 1000.0; // return distance in kilometers
		}
		#endregion
	}
}
