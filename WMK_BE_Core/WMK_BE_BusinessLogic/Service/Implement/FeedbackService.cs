﻿using Accord.Math;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.FeedbackModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.FeedbackModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class FeedbackService : IFeedbackService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public FeedbackService(IUnitOfWork unitOfWork , IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		#region get
		public async Task<ResponseObject<List<FeedbackResponse>>> Get(string? orderId)
		{
			var result = new ResponseObject<List<FeedbackResponse>>();
			var feedbackFoundList = await _unitOfWork.FeedbackRepository.GetAllAsync();//lay len va thao tac
			if ( orderId.IsNullOrEmpty() )//ko co tham so truyen vao
			{
				if ( feedbackFoundList.Count() > 0 )
				{
					var responseList = _mapper.Map<List<FeedbackResponse>>(feedbackFoundList);//response list cho method
					result.StatusCode = 200;
					result.Message = "Feedback list";
					result.Data = responseList;
					return result;
				}
				else
				{
					result.StatusCode = 400;
					result.Message = "Feedback list is empty";
					return result;
				}
			}
			else//orderId co nhap vao
			{
				var filterListOrder = feedbackFoundList.Where(f => f.OrderId.ToString() == orderId.ToLower()).ToList();//_unitOfWork.FeedbackRepository.Get(f => f.OrderId.ToString().Equals(orderId)).ToList();
				if ( filterListOrder.Count > 0 )
				{
					var responseList = _mapper.Map<List<FeedbackResponse>>(filterListOrder);//response list cho method
					result.StatusCode = 200;
					result.Message = "Feedback list";
					result.Data = responseList;
					return result;
				}
				else
				{
					result.StatusCode = 400;
					result.Message = "Feedback list with order ID: " + orderId + " is empty";
					return result;
				}
			}
		}
		#endregion

		#region create
		public async Task<ResponseObject<FeedbackResponse>> CreateFeedback(string userId , CreateFeedbackRequest request)
		{
			//kiem tra order ton tai
			//kiem tra order duoc hoan thanh chua
			//kiem tra user ton tai
			//kiem tra userId truyen vao co trung vs order dang duoc feedback ko
			//kiem tra feedback lien quan toi order do dang co chua
			// + chua thi tao moi
			// + co thi update feedback lien quan
			var result = new ResponseObject<FeedbackResponse>();
			var orderFound = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId.ToString());
			if ( orderFound == null )//kiem tra order ton tai
			{
				result.StatusCode = 500;
				result.Message = "Order not found";
				return result;
			}
			else //order co ton tai
			{
				if ( orderFound.Status == OrderStatus.Delivered || orderFound.Status == OrderStatus.Shipped )//order chua hoan thanh
				{
					var userFound = await _unitOfWork.UserRepository.GetByIdAsync(userId.ToString());
					if ( userFound == null )//kiem tra user ton tai
					{
						result.StatusCode = 500;
						result.Message = "User not found!";
						return result;
					}
					else
					{
						if ( !userId.Equals(orderFound.UserId.ToString()) )//user ko khop order
						{
							result.StatusCode = 500;
							result.Message = "User not match with order!";
							return result;
						}
						else//user khop order
						{//bat dau tao moi
							var feedbackFound = _unitOfWork.FeedbackRepository.Get(f => f.OrderId == orderFound.Id).ToList();
							if ( feedbackFound.Count == 0 )//chua co => tao moi
							{
								var newFeedback = _mapper.Map<Feedback>(request);
								newFeedback.CreatedAt = DateTime.UtcNow;
								if ( newFeedback.Description.IsNullOrEmpty() )//tao desciption mac dinh
								{
									newFeedback.Description = "None";
								}
								var createResult = await _unitOfWork.FeedbackRepository.CreateAsync(newFeedback);
								if ( createResult )
								{
									await _unitOfWork.CompleteAsync();
									result.StatusCode = 200;
									result.Message = "Create feedback Ok.";
									return result;
								}
								else
								{
									result.StatusCode = 500;
									result.Message = "Create feedback failed!";
									return result;
								}
							}
							else
							{
								result.StatusCode = 500;
								result.Message = "Feedback have exist!";
								return result;
							}
						}
					}
				}
				else //order chua hoan thanh
				{
					result.StatusCode = 500;
					result.Message = "Order not finished yet!";
					return result;
				}
			}
		}
		#endregion

		#region Update
		public async Task<ResponseObject<FeedbackResponse>> UpdateFeedback(Guid id , string userId , CreateFeedbackRequest request)
		{
			var result = new ResponseObject<FeedbackResponse>();
			var feedbackExist = await _unitOfWork.FeedbackRepository.GetByIdAsync(id.ToString());
			if ( feedbackExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Not have feedback!";
				return result;
			}
			var orderFound = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId.ToString());
			if ( orderFound == null )//kiem tra order ton tai
			{
				result.StatusCode = 500;
				result.Message = "Order not found";
				return result;
			}
			else
			{
				if ( orderFound.Status == OrderStatus.Delivered || orderFound.Status == OrderStatus.Shipped )//order đã hoàn thành
				{
					var userFound = await _unitOfWork.UserRepository.GetByIdAsync(userId.ToString());
					if ( userFound == null )//kiem tra user ton tai
					{
						result.StatusCode = 500;
						result.Message = "User not found!";
						return result;
					}
					else
					{
						if ( !userId.Equals(orderFound.UserId.ToString()) )//user ko khop order
						{
							result.StatusCode = 500;
							result.Message = "User not match with order!";
							return result;
						}
						else
						{
							//update order
							_mapper.Map(request , feedbackExist);
							var updateFeedbackRes = await _unitOfWork.FeedbackRepository.UpdateAsync(feedbackExist);
							if ( updateFeedbackRes )
							{
								result.StatusCode = 200;
								result.Message = "Update feedback success.";
								result.Data = _mapper.Map<FeedbackResponse>(feedbackExist);
								return result;
							}
							else
							{
								result.StatusCode = 500;
								result.Message = "Update feedback unsuccess!";
								return result;
							}
						}
					}
				}
				else //order chua hoan thanh
				{
					result.StatusCode = 500;
					result.Message = "Order not finished yet!";
					return result;
				}
			}
		}
		#endregion
	}
}