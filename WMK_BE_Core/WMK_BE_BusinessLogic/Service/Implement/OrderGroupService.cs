using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderGroupModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderGroupModel;
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
			if ( shipperExist != null && shipperExist.Role != Role.Shiper )
			{
				result.StatusCode = 403;
				result.Message = "Not have Shipper!";
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
				result.Message = "Not have staff!";
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
	}
}
