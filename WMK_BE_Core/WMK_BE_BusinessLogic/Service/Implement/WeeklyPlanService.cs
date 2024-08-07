﻿using AutoMapper;
using Azure.Core;
using Castle.Components.DictionaryAdapter.Xml;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.WeeklyPlanModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class WeeklyPlanService : IWeeklyPlanService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IRecipePlanService _recipePlanService;
		private readonly IMapper _mapper;
		private readonly IRedisService _redisService;

		#region Validator
		private readonly CreateWeeklyPlanValidator _createValidator;
		private readonly UpdateWeeklyPlanValidator _updateValidator;
		private readonly DeleteWeeklyPlanValidator _deleteValidator;
		private readonly ChangeStatusWeeklyPlanValidator _changeStatusValidator;
		#endregion
		public WeeklyPlanService(IUnitOfWork unitOfWork , IMapper mapper , IRecipePlanService recipePlanService , IRedisService redisService)
		{
			_unitOfWork = unitOfWork;
			_recipePlanService = recipePlanService;
			_mapper = mapper;
			_redisService = redisService;

			_createValidator = new CreateWeeklyPlanValidator();
			_updateValidator = new UpdateWeeklyPlanValidator();
			_deleteValidator = new DeleteWeeklyPlanValidator();
			_changeStatusValidator = new ChangeStatusWeeklyPlanValidator();
		}
		#region Get
		public async Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetAllFilterAsync(GetAllRequest? model)
		{
			var result = new ResponseObject<List<WeeklyPlanResponseModel>>();

			////ngày hiện tại
			//DateTime today = DateTime.UtcNow;

			////tìm ngày đầu tuần 
			//DateTime startWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

			////tìm ngày cuối tuần
			//DateTime endOfWeek = startWeek.AddDays(6);
			var weeklyPlans = new List<WeeklyPlan>();
			var weeklyPlanResponse = new List<WeeklyPlanResponseModel>();
			if ( model != null && (!model.Title.IsNullOrEmpty()
									|| (model.BeginDate != null)
										&& model.EndDate != null) )
			{
				if ( !model.Title.IsNullOrEmpty() )
				{
					var weeklyPLansByTitle = await GetWeeklyPlansByTitle(model.Title);
					if ( weeklyPLansByTitle != null && weeklyPLansByTitle.Data != null )
					{
						weeklyPlanResponse.AddRange(weeklyPLansByTitle.Data);
					}
				}
				if ( model.BeginDate != null && model.EndDate != null )
				{
					var weeklyPlansByDatetime = await GetWeeklyPlansByDatetime(model.BeginDate , model.EndDate);
					if ( weeklyPlansByDatetime != null && weeklyPlansByDatetime.Data != null )
					{
						weeklyPlanResponse.AddRange(weeklyPlansByDatetime.Data);
					}
				}
				// Loại bỏ các phần tử trùng lặp dựa trên Id
				weeklyPlanResponse = weeklyPlanResponse
					.GroupBy(c => c.Id)
					.Select(g => g.First())
					.ToList();
			}
			else
			{
				weeklyPlans = await _unitOfWork.WeeklyPlanRepository.GetAllAsync();
				weeklyPlanResponse = _mapper.Map<List<WeeklyPlanResponseModel>>(weeklyPlans);
			}
			if ( weeklyPlanResponse != null && weeklyPlanResponse.Any() )
			{
				foreach ( var item in weeklyPlanResponse )
				{
					Guid userId;
					if ( item.CreatedBy != null )
					{
						Guid.TryParse(item.CreatedBy , out userId);
						item.CreatedBy = _unitOfWork.UserRepository.GetUserNameById(userId);
					}
					else
					{
						item.CreatedBy = "UserName not found!";
					}
				}
				result.StatusCode = 200;
				result.Message = "Get weekly plan success (" + weeklyPlanResponse.Count() + ")";
				result.Data = weeklyPlanResponse;
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Don't have weekly pLan list";
			return result;
		}
		public async Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetWeeklyPlansByTitle(string title)
		{
			var result = new ResponseObject<List<WeeklyPlanResponseModel>>();
			var weeklyPlans = await _unitOfWork.WeeklyPlanRepository.GetAllAsync();
			weeklyPlans = weeklyPlans.Where(wp => wp.Title.ToLower().RemoveDiacritics().Contains(title.ToLower().RemoveDiacritics())).ToList();
			if ( weeklyPlans != null && weeklyPlans.Any() )
			{
				result.StatusCode = 200;
				result.Message = "List of weekly plans found by title";
				result.Data = _mapper.Map<List<WeeklyPlanResponseModel>>(weeklyPlans);
				return result;
			}
			result.StatusCode = 404;
			result.Message = "No weekly plans found with title!";
			return result;
		}
		public async Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetWeeklyPlansByDatetime(DateTime? beginDate , DateTime? endDate)
		{
			var result = new ResponseObject<List<WeeklyPlanResponseModel>>();

			var weeklyPlans = await _unitOfWork.WeeklyPlanRepository.GetAllAsync();

			var filteredPlans = weeklyPlans
				.Where(wp => wp.BeginDate >= beginDate && wp.EndDate <= endDate)
				.ToList();

			if ( filteredPlans.Any() )
			{
				result.StatusCode = 200;
				result.Message = "List of weekly plans found";
				result.Data = _mapper.Map<List<WeeklyPlanResponseModel>>(filteredPlans);
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "No weekly plans found within the specified dates!";
			}

			return result;
		}


		#endregion

		#region Create
		public async Task<ResponseObject<WeeklyPlanResponseModel>> CreateWeeklyPlanAsync(CreateWeeklyPlanRequest model , string createdBy)
		{
			var result = new ResponseObject<WeeklyPlanResponseModel>();
			try
			{
				var validateResult = _createValidator.Validate(model);
				if ( !validateResult.IsValid )
				{
					var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
					result.StatusCode = 400;
					result.Message = string.Join(" - " , error);
					return result;
				}
				//check user exist 
				var userExist = await _unitOfWork.UserRepository.GetByIdAsync(createdBy);
				if ( userExist == null )
				{
					result.StatusCode = 404;
					result.Message = "User not exist!";
					return result;
				}
				List<WeeklyPlan> currentList = await _unitOfWork.WeeklyPlanRepository.GetAllAsync();
				if ( currentList.Count() > 0 )
				{
					var foundDuplicate = currentList.Where(x => x.Title.Trim().Equals(model.Title.Trim())).FirstOrDefault();
					if ( foundDuplicate != null && (foundDuplicate.ProcessStatus == ProcessStatus.Processing
													|| foundDuplicate.ProcessStatus == ProcessStatus.Approved) )
					{
						result.StatusCode = 400;
						result.Message = "Weekly plan Title already existed";
						return result;
					}
				}
				//create weekly plan
				var newWeeklyPlan = _mapper.Map<WeeklyPlan>(model);
				newWeeklyPlan.CreateAt = DateTime.UtcNow;
				newWeeklyPlan.CreatedBy = createdBy;
				//check size of recipe
				if ( model.recipeIds.Count < 21 || model.recipeIds.Count > 200 )
				{
					result.StatusCode = 400;
					result.Message = "Must be 21 - 200 portion for each week";
					return result;
				}
				//add new weekly plan
				var createResult = await _unitOfWork.WeeklyPlanRepository.CreateAsync(newWeeklyPlan);
				if ( !createResult )
				{
					result.StatusCode = 500;
					result.Message = "Create weekly plan unsuccessfully!";
					return result;
				}
				await _unitOfWork.CompleteAsync();
				_unitOfWork.WeeklyPlanRepository.DetachEntity(newWeeklyPlan);
				//create list recipePlan
				var createRecipePlansResult = await _recipePlanService.CreateRecipePlanAsync(newWeeklyPlan.Id , model.recipeIds);
				if ( createRecipePlansResult.StatusCode == 200 && createRecipePlansResult.Data != null )
				{
					//assign the value of recipe plan to new weekly plan
					newWeeklyPlan.RecipePLans = createRecipePlansResult.Data;
					var updateWeeklyPLanResult = await _unitOfWork.WeeklyPlanRepository.UpdateAsync(newWeeklyPlan);
					if ( updateWeeklyPLanResult )
					{
						await _unitOfWork.CompleteAsync();
						result.StatusCode = createRecipePlansResult.StatusCode;
						result.Message = "Create Weekly plan successfully.";
						result.Data = _mapper.Map<WeeklyPlanResponseModel>(newWeeklyPlan);
						return result;
					}
					result.StatusCode = 500;
					result.Message = "Sign recipe plan with weekly plan unsuccess!";
					return result;
				}
				else
				{
					//delete weeklyPlan
					await _unitOfWork.WeeklyPlanRepository.DeleteAsync(newWeeklyPlan.Id.ToString());
					await _unitOfWork.CompleteAsync();
					result.StatusCode = createRecipePlansResult.StatusCode;
					result.Message = createRecipePlansResult.Message;
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

		#region create for customer
		public async Task<ResponseObject<WeeklyPlanResponseModel>> CreateForSutomer(CreateWeeklyPlanForCustomerRequest request)
		{   //kiem tra processStatus -> khong dung cho customer -> bao loi
			//kiem tra nguoi dung -> ko dung customer -> bao loi
			//kiem tra so luong weekPlan ma customer dang xet hien co -> du 5 thi bao loi. chua du 5 thi bat dau tao moi
			//so luong recipe vuot qua 200 thi bao loi ko cho tao qua 200
			//kiem tra thong tin cac mon an (recipeId) status la ko phai approved thi bao loi
			//bat dau tao recipePlan -> dung nhu ham Create ben tren. ke qua nhan ve loi thi xoa weekPlan vua tao va bao loi

			var result = new ResponseObject<WeeklyPlanResponseModel>();
			Guid idTo = new Guid();
			try//bat moi loi bat ngo va chu co message cu the 
			{
				if ( request.ProcessStatus != null && request.ProcessStatus != ProcessStatus.Customer )//kiem tra processStatus
				{
					result.StatusCode = 500;
					result.Message = "ProcessStatus not for customer";
					return result;
				}
				var userExist = await _unitOfWork.UserRepository.GetByIdAsync(request.CreatedBy);//kiem tra role cua nguoi tao (createBy), role phai la customer
				if ( userExist == null || userExist.Role != WMK_BE_RecipesAndPlans_DataAccess.Enums.Role.Customer )
				{
					result.StatusCode = 404;
					result.Message = "User not exist or not have access!";
					return result;
				}
				List<WeeklyPlan> currentList = await _unitOfWork.WeeklyPlanRepository.GetAllAsync();
				if ( currentList != null && currentList.Count() > 0 )
				{
					var foundDuplicate = currentList.FirstOrDefault(x => x.Title.Trim().Equals(request.Title.Trim()));
					if ( foundDuplicate != null && foundDuplicate.ProcessStatus == ProcessStatus.Customer )
					{
						result.StatusCode = 404;
						result.Message = "Weekly plan Title already existed";
						return result;
					}
					else
					{
						int countPlan = 0; //tinh xem dang co bao nhieu plan duoc tao boi nguoi dung roi. qua 5 thi ko cho tao them
						foreach ( var item in currentList )
						{
							if ( item.CreatedBy.Equals(request.CreatedBy , StringComparison.OrdinalIgnoreCase) && item.ProcessStatus == ProcessStatus.Customer )
							{
								countPlan++;
							}
						}
						if ( countPlan >= 5 )
						{
							result.StatusCode = 400;
							result.Message = "Reach limit in max personal plan. Remove some personal plan before create new. ";
							return result;
						}
						WeeklyPlan newOne = _mapper.Map<WeeklyPlan>(request);
						newOne.CreateAt = DateTime.UtcNow;
						var createResult = await _unitOfWork.WeeklyPlanRepository.CreateAsync(newOne);
						if ( createResult )
						{
							await _unitOfWork.CompleteAsync();
							idTo = newOne.Id;
							var createRecipePlanResult = await _recipePlanService.CreateRecipePlanAsync(newOne.Id , request.recipeIds);
							if ( createRecipePlanResult.StatusCode == 200 && createRecipePlanResult.Data != null )
							{
								result.StatusCode = 200;
								result.Message = "OK";
								return result;
							}
							await _unitOfWork.WeeklyPlanRepository.DeleteAsync(newOne.Id.ToString()); //xoa thong tin vi ko tao dc
							await _unitOfWork.CompleteAsync();
							result.StatusCode = createRecipePlanResult.StatusCode;
							result.Message = createRecipePlanResult.Message;
							return result;
						}
						else
						{
							result.StatusCode = 500;
							result.Message = "Error at create for customer - weekplan service";
							return result;
						}
					}
				}
				else
				{
					result.StatusCode = 400;
					result.Message = "Can not create personal plan at this time. Standard weekly Plan have no data";
					return result;
				}
			}
			catch ( Exception ex )
			{
				await _unitOfWork.WeeklyPlanRepository.DeleteAsync(idTo.ToString()); //xoa thong tin vi ko tao dc
				result.StatusCode = 500;
				result.Message = ex.Message;
				return result;
			}
		}
		#endregion

		#region get all

		public async Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetAllWeeklyPLanAsync(string? name)
		{
			var result = new ResponseObject<List<WeeklyPlanResponseModel>>();

			var weeklyPLans = await _unitOfWork.WeeklyPlanRepository.GetAllAsync();
			if ( name != null )
			{
				weeklyPLans = weeklyPLans.Where(wp =>
				wp.Title.ToLower().Trim().Contains(name.ToLower().Trim())).ToList();
			}
			result.StatusCode = 200;
			result.Message = "Success list weekly pLan: ";
			result.Data = _mapper.Map<List<WeeklyPlanResponseModel>>(weeklyPLans);
			return result;


		}

		public async Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetAllAsync(string name = "")
		{
			var result = new ResponseObject<List<WeeklyPlanResponseModel>>();

			//get from redis
			var redisKey = "WeeklyPlanList";
			var redisData = await _redisService.GetValueAsync<List<WeeklyPlanResponseModel>>(redisKey);
			if ( redisData != null && redisData.Count > 0 )
			{
				result.StatusCode = 200;
				result.Message = "WeeklyPlan list: " + redisData.Count();
				result.Data = redisData;
				return result;
			}

			var weeklyPlans = _unitOfWork.WeeklyPlanRepository.Get(x => x.ProcessStatus == ProcessStatus.Approved).ToList();
			var returnList = weeklyPlans.Where(x => x.Title.ToLower().RemoveDiacritics().Contains(name.ToLower().RemoveDiacritics())).ToList();

			if ( weeklyPlans != null && weeklyPlans.Count > 0 )
			{
				var returnResult = _mapper.Map<List<WeeklyPlanResponseModel>>(returnList);
				foreach ( var item in returnResult )
				{
					var userCreate = await _unitOfWork.UserRepository.GetByIdAsync(item.CreatedBy.ToString());
					if ( item.UpdatedBy != null )
					{
						var userUpdate = await _unitOfWork.UserRepository.GetByIdAsync(item.UpdatedBy.ToString());
					}
					if ( item.ApprovedBy != null )
					{
						var userApprove = await _unitOfWork.UserRepository.GetByIdAsync(item.ApprovedBy.ToString());
					}
					if ( userCreate != null )
					{
						item.CreatedBy = userCreate.FirstName + " " + userCreate.LastName;
					}
				}
				result.StatusCode = 200;
				result.Message = "WeeklyPlan list: " + returnResult.Count();
				result.Data = returnResult;

				//set cache to redis
				await _redisService.SetValueAsync(redisKey , returnResult , TimeSpan.FromDays(3));

				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Not have plan!";
				return result;
			}
		}


		#endregion

		#region get by id

		public async Task<ResponseObject<WeeklyPlanResponseModel?>> GetByIdAsync(Guid id)
		{
			var result = new ResponseObject<WeeklyPlanResponseModel?>();
			var weeklyPlanExist = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(id.ToString());
			if ( weeklyPlanExist != null )
			{
				var weeklyPlan = _mapper.Map<WeeklyPlanResponseModel>(weeklyPlanExist);
				Guid idConvert;
				if ( weeklyPlan.CreatedBy != null )
				{
					Guid.TryParse(weeklyPlan.CreatedBy , out idConvert);
					weeklyPlan.CreatedBy = _unitOfWork.UserRepository.GetUserNameById(idConvert);
				}
				else
				{
					weeklyPlan.CreatedBy = "UserName not found!";
				}
				result.StatusCode = 200;
				result.Message = "Get weekly plan success";
				result.Data = weeklyPlan;
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Weekly plan not exist!";
				return result;
			}
		}


		#endregion

		#region update
		public async Task<ResponseObject<WeeklyPlanResponseModel>> UpdateWeeklyPlanAsync(Guid id , UpdateWeeklyPlanRequestModel model)
		{
			var result = new ResponseObject<WeeklyPlanResponseModel>();
			try
			{
				var validateResult = _updateValidator.Validate(model);
				if ( !validateResult.IsValid )
				{
					var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
					result.StatusCode = 400;
					result.Message = string.Join(" - " , error);
					return result;
				}
				//trường hợp recipe ko nằm trong quy định từ 21 - 200 món
				if ( model.recipeIds != null && model.recipeIds.Count > 200 && model.recipeIds.Count < 21 )
				{
					result.StatusCode = 400;
					result.Message = "Must be 21 - 200 portion for each week";
					return result;
				}
				else
				{
					var foundWeeklyPlan = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(id.ToString());
					if ( foundWeeklyPlan == null )
					{
						result.StatusCode = 404;
						result.Message = "Weekly plan not exist!";
						return result;
					}
					//bat dau thay doi thong tin cho week plan
					_unitOfWork.WeeklyPlanRepository.DetachEntity(foundWeeklyPlan);
					_mapper.Map(model , foundWeeklyPlan);
					foundWeeklyPlan.ProcessStatus = ProcessStatus.Processing;//chuyển sang thì duyệt lại nếu là admin hoặc manager
																			 //thì tự duyệt còn staff thì phải qua manager duyệt 

					//trường hợp check xem nếu cùng bữa ăn thì không được trùng recipe 
					//tìm xem recipe plan có trước đó và xóa đi
					var relatedRecipePlans = _unitOfWork.RecipePlanRepository.Get(x => x.StandardWeeklyPlanId.ToString().ToLower()
																						.Equals(id.ToString().ToLower())).ToList();
					if ( relatedRecipePlans != null && relatedRecipePlans.Any() )
					{
						_unitOfWork.RecipePlanRepository.RemoveRange(relatedRecipePlans);
						await _unitOfWork.CompleteAsync();
					}
					else
					{
						result.StatusCode = 500;
						result.Message = "Remove recipe plan faild!";
						return result;
					}
					if ( model.recipeIds != null && model.recipeIds.Any() )
					{
						var createRecipePlansResult = await _recipePlanService.CreateRecipePlanAsync(id , model.recipeIds);
						if ( createRecipePlansResult.StatusCode == 200 && createRecipePlansResult.Data != null )
						{
							await _unitOfWork.CompleteAsync(); //sau khi xac dinh da tao duoc ban cap nhat roi thi xoa di ban cap nhat cu 
							result.StatusCode = 200;
							result.Message = "Update Weekly plan successfully.";
							return result;
						}
						else//neu khong duoc thi ko luu gi het - ko dung ham completeAsync nen ko luu ket qua
						{
							result.StatusCode = createRecipePlansResult.StatusCode;
							result.Message = createRecipePlansResult.Message;
							return result;
						}
					}
					else
					{
						result.StatusCode = 404;
						result.Message = "Recipe can't not empty! Please input recipe!!";
						return result;
					}
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

		#region Delete
		public async Task<ResponseObject<WeeklyPlanResponseModel>> DeleteWeeklyPlanAsync(Guid id)
		{
			var result = new ResponseObject<WeeklyPlanResponseModel>();
			//var validateResult = _deleteValidator.Validate(model);
			//if ( !validateResult.IsValid )
			//{
			//	var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
			//	result.StatusCode = 400;
			//	result.Message = string.Join(" - " , error);
			//	return result;
			//}
			var weeklyPlanExist = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(id.ToString());
			if ( weeklyPlanExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Weekly plan not exist!";
				return result;
			}
			else
			{
				var deleteResult = await _unitOfWork.WeeklyPlanRepository.DeleteAsync(weeklyPlanExist.Id.ToString());
				if ( deleteResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Delete weekly plan with successfullly";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Delete weekly plan with unsuccessfullly!";
					return result;
				}
			}
		}

		#endregion

		#region Change status
		public async Task<ResponseObject<WeeklyPlanResponseModel>> ChangeStatusWeeklyPlanAsync(string? userId , Guid id , ChangeStatusWeeklyPlanRequest model)
		{
			var result = new ResponseObject<WeeklyPlanResponseModel>();
			var validateResult = _changeStatusValidator.Validate(model);
			if ( !validateResult.IsValid )
			{
				var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			//nếu model.ProcessStatus == Approved hoặc Denied thì chỉ được manager hoặc admin đổi
			if ( model.ProcessStatus == ProcessStatus.Approved || model.ProcessStatus == ProcessStatus.Denied )
			{
				var userExist = await _unitOfWork.UserRepository.GetByIdAsync(userId);
				if ( userExist != null && (userExist.Role == WMK_BE_RecipesAndPlans_DataAccess.Enums.Role.Staff
											|| userExist.Role == WMK_BE_RecipesAndPlans_DataAccess.Enums.Role.Shipper
											|| userExist.Role == WMK_BE_RecipesAndPlans_DataAccess.Enums.Role.Customer) )
				{
					result.StatusCode = 400;
					result.Message = "Not have permission!";
					return result;
				}
			}
			var weeklyPlanExist = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(id.ToString());
			if ( weeklyPlanExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Weekly plan not exist!";
				return result;
			}

			//nếu status là customer thì không cần đổi status
			if ( weeklyPlanExist.ProcessStatus == ProcessStatus.Customer )
			{
				result.StatusCode = 400;
				result.Message = "Not have permission!";
				return result;
			}

			weeklyPlanExist.Notice = model.Notice;
			weeklyPlanExist.ProcessStatus = model.ProcessStatus;
			var changeResult = await _unitOfWork.WeeklyPlanRepository.UpdateAsync(weeklyPlanExist);
			if ( changeResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Change status weekly plan to (" + weeklyPlanExist.ProcessStatus + ") successfully";
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Change status weekly plan to(" + weeklyPlanExist.ProcessStatus + ") unsuccessfully!";
				return result;
			}
		}
		#endregion

		#region get week plan list with user id
		public async Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetListByCustomerId(Guid customerId)
		{
			var result = new ResponseObject<List<WeeklyPlanResponseModel>>();
			try
			{
				//tìm list đc tạo bởi ng dùng và nó chưa bị cancel (process status còn là customer)
				var foundList = _unitOfWork.WeeklyPlanRepository.Get(x => x.CreatedBy.ToLower().Equals(customerId.ToString().ToLower()) && x.ProcessStatus == ProcessStatus.Customer).ToList();
				if ( foundList.Count() == 0 ) //ko tim dc
				{
					result.StatusCode = 500;
					result.Message = "Not found with user id";
					return result;
				}
				else //co thong tin
				{
					var returnList = _mapper.Map<List<WeeklyPlanResponseModel>>(foundList);
					//gan ten cho creatdBy va approvedBy
					foreach ( var item in returnList )
					{
						string userName = null;
						Guid idConvert;
						//tim ten cho CreatedBy
						if ( item.CreatedBy != null )
						{
							Guid.TryParse(item.CreatedBy , out idConvert);
							userName = _unitOfWork.UserRepository.GetUserNameById(idConvert);
						}
						if ( userName != null )
						{
							item.CreatedBy = userName;
						}
						//tim ten cho approvedBy
						if ( item.ApprovedBy != null )
						{
							Guid.TryParse(item.ApprovedBy , out idConvert);
							userName = _unitOfWork.UserRepository.GetUserNameById(idConvert);
						}
						if ( userName != null )
						{
							item.ApprovedBy = userName;
						}
					}
					result.StatusCode = 200;
					result.Message = "Ok";
					result.Data = returnList;
					return result;
				}
				//}
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
				return result;
			}
		}
		#endregion

		#region update - full info
		public async Task<ResponseObject<WeeklyPlanResponseModel>> UpdateFullInfo(Guid id , UpdateWeeklyPlanRequest request) //ham nay se dung cho ca viec update thong tin co ban hoac ca thog tin cu the cua weekPlan
		{
			var result = new ResponseObject<WeeklyPlanResponseModel>();
			try
			{
				//tim thong tin cua weeklyPlan - ok
				//cap nhat thong tin co ban cho week plan do
				//tim thong tin cho tat ra recipePlan lien quan - ok
				//xoa het thong tin - ok
				//tao lai thong tin moi - ok
				if ( request.recipeIds != null && request.recipeIds.Count > 200 )
				{
					result.StatusCode = 400;
					result.Message = "Vuot qua pham vi cho phep. dat toi da duoi 200 cong thuc";
					return result;
				}
				else
				{
					var foundWeeklyPlan = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(id.ToString());
					if ( foundWeeklyPlan == null )
					{
						result.Message = "Not found week plan";
						return result;
					}
					else//bat dau tim recipePlans
					{
						//bat dau thay doi thong tin cho week plan
						_unitOfWork.WeeklyPlanRepository.DetachEntity(foundWeeklyPlan);
						_mapper.Map(request , foundWeeklyPlan);
						var updateWeeklyPlanResult = await _unitOfWork.WeeklyPlanRepository.UpdateAsync(foundWeeklyPlan);
						if ( request.recipeIds != null && request.recipeIds.Any() )//kiem tra neu thong tin cu the co dinh kem khong
						{
							if ( updateWeeklyPlanResult ) //update thanh cong
							{
								var relatedRecipePlans = _unitOfWork.RecipePlanRepository.Get(x => x.StandardWeeklyPlanId.ToString().ToLower().Equals(id.ToString().ToLower())).ToList();
								if ( relatedRecipePlans != null )
								{
									foreach ( var item in relatedRecipePlans.ToList() )
									{
										await _unitOfWork.RecipePlanRepository.DeleteAsync(item.Id.ToString()); //co the su dung removeRange - can tim hieu them
																												//await _unitOfWork.CompleteAsync();
									}
								}
								else //bao loi ko tim thay gi het - cho nay co the cai tien cho thanh 1 ham vua tao moi vua cap nhat duoc. neu co cai tien thi duoiday la phan tao moi
								{
									result.Message = "Not found any existed to update";
									return result;
								}
								//bat dau tao recipePlan moi tu day
								var createRecipePlansResult = await _recipePlanService.CreateRecipePlanAsync(id , request.recipeIds);
								if ( createRecipePlansResult.StatusCode == 200 && createRecipePlansResult.Data != null )
								{

									await _unitOfWork.CompleteAsync(); //sau khi xac dinh da tao duoc ban cap nhat roi thi xoa di ban cap nhat cu 
									result.StatusCode = 200;
									result.Message = "Update Weekly plan successfully.";
									return result;
								}
								else//neu khong duoc thi ko luu gi het - ko dung ham completeAsync nen ko luu ket qua
								{
									result.StatusCode = createRecipePlansResult.StatusCode;
									result.Message = createRecipePlansResult.Message;
									return result;
								}
							}
							else //update khong thanh cong -> bao loi
							{
								result.Message = "Update failed";
								return result;
							}
						}
						else //chi update lai thong tin co ban cua week plan ma khong dung toi thong tin cu the
						{
							if ( updateWeeklyPlanResult )
							{
								await _unitOfWork.CompleteAsync();
								result.StatusCode = 200;
								result.Message = "Update Weekly plan successfully.";
								return result;
							}
							else
							{
								result.StatusCode = 500;
								result.Message = "Update failed";
								return result;
							}
						}

					}
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
	}
}
