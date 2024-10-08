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
		//private readonly IRedisService _redisService;

		#region Validator
		private readonly CreateWeeklyPlanValidator _createValidator;
		private readonly UpdateWeeklyPlanValidator _updateValidator;
		private readonly DeleteWeeklyPlanValidator _deleteValidator;
		private readonly ChangeStatusWeeklyPlanValidator _changeStatusValidator;
		#endregion
		public WeeklyPlanService(IUnitOfWork unitOfWork , IMapper mapper , IRecipePlanService recipePlanService)
		{
			_unitOfWork = unitOfWork;
			_recipePlanService = recipePlanService;
			_mapper = mapper;
			//_redisService = redisService;

			_createValidator = new CreateWeeklyPlanValidator();
			_updateValidator = new UpdateWeeklyPlanValidator();
			_deleteValidator = new DeleteWeeklyPlanValidator();
			_changeStatusValidator = new ChangeStatusWeeklyPlanValidator();
		}
		#region Get All for Administrator
		public async Task<ResponseObject<List<WeeklyPlanResponseModelForWeb>>> GetAllFilterAsync(GetAllRequest? model)
		{
			var result = new ResponseObject<List<WeeklyPlanResponseModelForWeb>>();

			////ngày hiện tại
			//DateTime today = DateTime.UtcNow.AddHours(7);

			////tìm ngày đầu tuần 
			//DateTime startWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

			////tìm ngày cuối tuần
			//DateTime endOfWeek = startWeek.AddDays(6);
			var weeklyPlans = new List<WeeklyPlan>();
			var weeklyPlanResponse = new List<WeeklyPlanResponseModelForWeb>();
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
				weeklyPlanResponse = _mapper.Map<List<WeeklyPlanResponseModelForWeb>>(weeklyPlans);
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
				result.Message = "Tìm thấy";
				result.Data = weeklyPlanResponse.OrderBy(wp => wp.ProcessStatus).ToList();
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Không tìm thấy";
			result.Data = [];
			return result;
		}
		#region mini search
		public async Task<ResponseObject<List<WeeklyPlanResponseModelForWeb>>> GetWeeklyPlansByTitle(string title)
		{
			var result = new ResponseObject<List<WeeklyPlanResponseModelForWeb>>();
			var weeklyPlans = await _unitOfWork.WeeklyPlanRepository.GetAllAsync();
			weeklyPlans = weeklyPlans.Where(wp => wp.Title.ToLower().RemoveDiacritics().Contains(title.ToLower().RemoveDiacritics())).ToList();
			if ( weeklyPlans != null && weeklyPlans.Any() )
			{
				result.StatusCode = 200;
				result.Message = "Tìm thấy";
				result.Data = _mapper.Map<List<WeeklyPlanResponseModelForWeb>>(weeklyPlans);
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Không có dữ liệu";
			return result;
		}
		public async Task<ResponseObject<List<WeeklyPlanResponseModelForWeb>>> GetWeeklyPlansByDatetime(DateTime? beginDate , DateTime? endDate)
		{
			var result = new ResponseObject<List<WeeklyPlanResponseModelForWeb>>();

			var weeklyPlans = await _unitOfWork.WeeklyPlanRepository.GetAllAsync();

			var filteredPlans = weeklyPlans
				.Where(wp => wp.BeginDate >= beginDate && wp.EndDate <= endDate)
				.ToList();

			if ( filteredPlans.Any() )
			{
				result.StatusCode = 200;
				result.Message = "Tìm thấy";
				result.Data = _mapper.Map<List<WeeklyPlanResponseModelForWeb>>(filteredPlans);
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Không có dữ liệu";
			}

			return result;
		}
		#endregion mini search

		#endregion

		#region get all for customer
		public async Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetAllAsync(string name = "")
		{
			var result = new ResponseObject<List<WeeklyPlanResponseModel>>();

			////get from redis
			//var redisKey = "WeeklyPlanList";
			//var redisData = await _redisService.GetValueAsync<List<WeeklyPlanResponseModel>>(redisKey);
			//if ( redisData != null && redisData.Count > 0 )
			//{
			//	result.StatusCode = 200;
			//	result.Message = "WeeklyPlan list: " + redisData.Count();
			//	result.Data = redisData;
			//	return result;
			//}
			var listsearch = name.Split(",");

			var weeklyPlans = _unitOfWork.WeeklyPlanRepository.Get(x => x.ProcessStatus == ProcessStatus.Approved).ToList();
			var listTemp = new List<WeeklyPlan>();
			if ( name.IsNullOrEmpty() )
			{
				listTemp = weeklyPlans;
			}
			foreach ( var item in listsearch )
			{
				foreach ( var weeklyPlan in weeklyPlans )
				{
					var recipePlans = weeklyPlan.RecipePLans;
					foreach ( var recipePlan in recipePlans )
					{
						var recipe = recipePlan.Recipe;
						if ( recipe.RecipeCategories.Select(x => x.Category.Name).Contains(item) )
						{
							listTemp.Add(weeklyPlan);
						}

					}
				}
			}

			var returnList = listTemp.Distinct();

			if ( weeklyPlans != null && weeklyPlans.Count > 0 )
			{
				var returnResult = _mapper.Map<List<WeeklyPlanResponseModel>>(returnList);
				foreach ( var item in returnResult )
				{
					var userCreate = await _unitOfWork.UserRepository.GetByIdAsync(item.CreatedBy.ToString());
					if ( userCreate != null )
					{
						item.CreatedBy = userCreate.FirstName + " " + userCreate.LastName;
					}
				}
				result.StatusCode = 200;
				result.Message = "Tìm thấy: " + returnResult.Count();
				result.Data = returnResult;

				////set cache to redis
				//await _redisService.SetValueAsync(redisKey , returnResult , TimeSpan.FromDays(3));

				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Không có dữ liệu";
				return result;
			}
		}


		#endregion

		#region get week plan list with user id
		public Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetListByCustomerId(Guid customerId)
		{
			var result = new ResponseObject<List<WeeklyPlanResponseModel>>();
			try
			{
				//tìm list đc tạo bởi ng dùng và nó chưa bị cancel (process status còn là customer)
				var foundList = _unitOfWork.WeeklyPlanRepository.Get(x => x.CreatedBy.ToLower().Equals(customerId.ToString().ToLower())
																		&& x.ProcessStatus == ProcessStatus.Customer).ToList();
				if ( foundList.Count() == 0 ) //ko tim dc
				{
					result.StatusCode = 500;
					result.Message = "Không tìm thấy";
					return Task.FromResult(result);
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
					}
					result.StatusCode = 200;
					result.Message = "Ok";
					result.Data = returnList;
					return Task.FromResult(result);
				}
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
				return Task.FromResult(result);
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
				result.Message = "Tìm thấy";
				result.Data = weeklyPlan;
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Không tìm thấy weekplan!";
				return result;
			}
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
					result.Message = "Không tìm thấy người dùng";
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
						result.Message = "Trùng tên";
						return result;
					}
				}
				//create weekly plan
				var newWeeklyPlan = _mapper.Map<WeeklyPlan>(model);
				newWeeklyPlan.CreateAt = DateTime.UtcNow.AddHours(7);
				newWeeklyPlan.CreatedBy = createdBy;
				newWeeklyPlan.BaseStatus = BaseStatus.UnAvailable;

				List<Guid> recipeIdListCheck = new List<Guid>();
				foreach ( var recipePlan in model.recipeIds )
				{
					recipeIdListCheck.Add(recipePlan.recipeId);
				}
				if ( recipeIdListCheck.Distinct().Count() < 10 )////check list recipe truyen vao phai co it nhat 10 recipe khac nhau
				{
					result.StatusCode = 400;
					result.Message = "Phải có ít nhất 10 món khác nhau cho 1 tuần";
					return result;
				}
				//add new weekly plan
				var createResult = await _unitOfWork.WeeklyPlanRepository.CreateAsync(newWeeklyPlan);
				if ( !createResult )
				{
					await _unitOfWork.WeeklyPlanRepository.DeleteAsync(newWeeklyPlan.Id.ToString());
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 500;
					result.Message = "Tạo mới không thành công";
					return result;
				}
				await _unitOfWork.CompleteAsync();
				//_unitOfWork.WeeklyPlanRepository.DetachEntity(newWeeklyPlan);
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
						result.Message = "Tạo thành công.";
						result.Data = _mapper.Map<WeeklyPlanResponseModel>(newWeeklyPlan);
						return result;
					}
					await _unitOfWork.WeeklyPlanRepository.DeleteAsync(newWeeklyPlan.Id.ToString());
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 500;
					result.Message = "Tạo không thành công";
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
			try//bat moi loi bat ngo va chua co message cu the 
			{
				if ( request.ProcessStatus != null && request.ProcessStatus != ProcessStatus.Customer )//kiem tra processStatus
				{
					result.StatusCode = 500;
					result.Message = "ProcessStatus không phải của khách hàng";
					return result;
				}
				var userExist = await _unitOfWork.UserRepository.GetByIdAsync(request.CreatedBy);//kiem tra role cua nguoi tao (createBy), role phai la customer
				if ( userExist == null || userExist.Role != WMK_BE_RecipesAndPlans_DataAccess.Enums.Role.Customer )
				{
					result.StatusCode = 404;
					result.Message = "Người dùng không tồn tại hoặc không có quyền thực thi!";
					return result;
				}
				List<WeeklyPlan> currentList = await _unitOfWork.WeeklyPlanRepository.GetAllAsync();
				if (currentList != null && currentList.Count() > 0)
				{
					//var foundDuplicate = currentList.FirstOrDefault(x => x.Title.Trim().Equals(request.Title.Trim()));
					//if ( foundDuplicate != null && foundDuplicate.ProcessStatus == ProcessStatus.Customer )
					//{
					//	result.StatusCode = 404;
					//	result.Message = "Trùng title ";
					//	return result;
					//}
					//else
					//{
					int countPlan = 0; //tinh xem dang co bao nhieu plan duoc tao boi nguoi dung roi. qua 5 thi ko cho tao them
					foreach (var item in currentList)
					{
						if (item.CreatedBy.Equals(request.CreatedBy, StringComparison.OrdinalIgnoreCase) && item.ProcessStatus == ProcessStatus.Customer)
						{
							countPlan++;
						}
					}
					if (countPlan >= 5)
					{
						result.StatusCode = 400;
						result.Message = "Đạt giới hạn kế hoạch cá nhân là 5. Vui lòng hủy bớt kế hoạch cá nhân để có thể tạo thêm ";
						return result;
					}
					var referenceWP = currentList.Where(wp => wp.Id.ToString().ToLower().Equals(request.WeeklyPlanId.ToLower())).FirstOrDefault();
					WeeklyPlan newOne = _mapper.Map<WeeklyPlan>(request);
					newOne.BaseStatus = referenceWP.BaseStatus;
					newOne.CreateAt = DateTime.UtcNow.AddHours(7);
					var createResult = await _unitOfWork.WeeklyPlanRepository.CreateAsync(newOne);
					if (createResult)
					{
						await _unitOfWork.CompleteAsync();
						idTo = newOne.Id;
						var createRecipePlanResult = await _recipePlanService.CreateRecipePlanAsync(newOne.Id, request.recipeIds);
						if (createRecipePlanResult.StatusCode == 200 && createRecipePlanResult.Data != null)
						{
							await _unitOfWork.CompleteAsync();
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
						result.Message = "Tạo kế hoạch cá nhân thất bại. Vui lòng liên hệ bộ phận chăm sóc khách hàng để được hỗ trợ";
						return result;
					}
				//}
				}
				else
				{
					result.StatusCode = 400;
					result.Message = "Không thể tạo kế hoạch cá nhân, danh sách trống";
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
				List<Guid> recipeIdListCheck = new List<Guid>();
				foreach ( var recipePlan in model.recipeIds )
				{
					recipeIdListCheck.Add(recipePlan.recipeId);
				}
				if ( recipeIdListCheck.Distinct().Count() < 10 )//check list recipe truyen vao phai co it nhat 10 recipe khac nhau
				{
					result.StatusCode = 400;
					result.Message = "Phải có ít nhất 10 món khác nhau cho 1 tuần";
					return result;
				}
				else
				{
					var foundWeeklyPlan = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(id.ToString());
					if ( foundWeeklyPlan == null )
					{
						result.StatusCode = 404;
						result.Message = "Weekly plan không tồn tại";
						return result;
					}
					//bat dau thay doi thong tin cho week plan
					_unitOfWork.WeeklyPlanRepository.DetachEntity(foundWeeklyPlan);
					_mapper.Map(model , foundWeeklyPlan);
					foundWeeklyPlan.ProcessStatus = ProcessStatus.Processing;//chuyển sang thì duyệt lại nếu là admin hoặc manager
																			 //thì tự duyệt còn staff thì phải qua manager duyệt 
					foundWeeklyPlan.BaseStatus = BaseStatus.UnAvailable;
					var updateWeekPlanResult = await _unitOfWork.WeeklyPlanRepository.UpdateAsync(foundWeeklyPlan);
					if ( updateWeekPlanResult )
					{
						//tìm xem recipe plan có trước đó và xóa đi
						var relatedRecipePlans = _unitOfWork.RecipePlanRepository.Get(x => x.StandardWeeklyPlanId.ToString().ToLower()
																							.Equals(id.ToString().ToLower())).ToList();
						if ( relatedRecipePlans != null && relatedRecipePlans.Any() )
						{
							_unitOfWork.RecipePlanRepository.RemoveRange(relatedRecipePlans);
							await _unitOfWork.CompleteAsync();
						}
						if ( model.recipeIds != null && model.recipeIds.Any() )
						{
							var createRecipePlansResult = await _recipePlanService.CreateRecipePlanAsync(id , model.recipeIds);
							//cap nhat thong tin cho weekplan - Danh
							if ( createRecipePlansResult.StatusCode == 200 && createRecipePlansResult.Data != null )
							{
								await _unitOfWork.CompleteAsync(); //sau khi xac dinh da tao duoc ban cap nhat roi thi xoa di ban cap nhat cu 
								result.StatusCode = 200;
								result.Message = "Cập nhật thành công.";
								return result;
							}
							else//neu khong duoc thi ko luu gi het - ko dung ham completeAsync nen ko luu ket qua
							{
								result.StatusCode = 500;
								result.Message = createRecipePlansResult.Message + " Cập nhật thất bại";
								return result;
							}
						}
						else
						{
							result.StatusCode = 404;
							result.Message = "Thông tin món ăn không để trống";
							return result;
						}
					}
					else
					{
						result.StatusCode = 500;
						result.Message = "Cập nhật thông tin cơ bản kông thành công";
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
				result.Message = "Weekly plan không tồn tại!";
				return result;
			}
			else
			{
				var deleteResult = await _unitOfWork.WeeklyPlanRepository.DeleteAsync(weeklyPlanExist.Id.ToString());
				if ( deleteResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Xóa thành công";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Xóa không thành công!";
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
					result.Message = "Không có quyền thực thi!";
					return result;
				}
			}
			var weeklyPlanExist = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(id.ToString());
			if ( weeklyPlanExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Kế hoạch không tồn tại!";
				return result;
			}

			//nếu status là customer thì không cần đổi status
			if ( weeklyPlanExist.ProcessStatus == ProcessStatus.Customer )
			{
				result.StatusCode = 400;
				result.Message = "Không có quyền thực thi!";
				return result;
			}
			weeklyPlanExist.Notice = model.Notice;
			//nếu đổi processStatus sang denied thì base sẽ đổi sang unavailable
			if ( model.ProcessStatus == ProcessStatus.Denied )
			{
				weeklyPlanExist.BaseStatus = BaseStatus.UnAvailable;
			}
			if ( model.ProcessStatus == ProcessStatus.Approved )
			{
				weeklyPlanExist.BaseStatus = BaseStatus.Available;
			}
			weeklyPlanExist.ProcessStatus = model.ProcessStatus;
			var changeResult = await _unitOfWork.WeeklyPlanRepository.UpdateAsync(weeklyPlanExist);
			if ( changeResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Đổi trạng thái  (" + weeklyPlanExist.ProcessStatus + ") thành công";
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Đổi trạng thái(" + weeklyPlanExist.ProcessStatus + ") không thành công!";
				return result;
			}
		}
		public async Task<ResponseObject<WeeklyPlanResponseModel>> ChangeBaseStatusWeeklyPlanAsync(Guid id , ChangeBaseStatusWeeklyPlanRequest model)
		{
			var result = new ResponseObject<WeeklyPlanResponseModel>();

			var weeklyPlanExist = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(id.ToString());
			if ( weeklyPlanExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Không tìm thấy!";
				return result;
			}
			if ( weeklyPlanExist.ProcessStatus == ProcessStatus.Processing )
			{
				result.StatusCode = 400;
				result.Message = "Weekplan đang chò xử lí.";
				return result;
			}
			weeklyPlanExist.BaseStatus = model.BaseStatus;
			var updateWeeklyPlanResult = await _unitOfWork.WeeklyPlanRepository.UpdateAsync(weeklyPlanExist);
			if ( updateWeeklyPlanResult )
			{
				result.StatusCode = 200;
				result.Message = "Đổi trạng thái thành công!";
				//result.Data = _mapper.Map<WeeklyPlanResponseModel>(weeklyPlanExist);
				return result;
			}
			result.StatusCode = 500;
			result.Message = "Đổi trạng thái thành công!";
			return result;

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
					result.Message = "Vượt quá phạm vi cho phép 200 công thức";
					return result;
				}
				else
				{
					var foundWeeklyPlan = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(id.ToString());
					if ( foundWeeklyPlan == null )
					{
						result.Message = "Không tìm thấy Week plan";
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
									result.Message = "Không tìm thấy thông tin cần cập nhật";
									return result;
								}
								//bat dau tao recipePlan moi tu day
								var createRecipePlansResult = await _recipePlanService.CreateRecipePlanAsync(id , request.recipeIds);
								if ( createRecipePlansResult.StatusCode == 200 && createRecipePlansResult.Data != null )
								{

									await _unitOfWork.CompleteAsync(); //sau khi xac dinh da tao duoc ban cap nhat roi thi xoa di ban cap nhat cu 
									result.StatusCode = 200;
									result.Message = "Cập nhật thành công";
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
								result.Message = "Cập nhật không thành công";
								return result;
							}
						}
						else //chi update lai thong tin co ban cua week plan ma khong dung toi thong tin cu the
						{
							if ( updateWeeklyPlanResult )
							{
								await _unitOfWork.CompleteAsync();
								result.StatusCode = 200;
								result.Message = "Cập nhật thành công.";
								return result;
							}
							else
							{
								result.StatusCode = 500;
								result.Message = "Cập nhật không thành công ";
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

		public async Task<ResponseObject<WeeklyPlanResponseModel>> OnOffOrderAsync(bool status)
		{
			var result = new ResponseObject<WeeklyPlanResponseModel>();
			//chỉ lấy wp có status là cus và approved và customer để đổi còn lại (denied, cancel, processing,...) sẽ giữ lại process status
			var weeklyPlans = _unitOfWork.WeeklyPlanRepository.Get(x => x.ProcessStatus == ProcessStatus.Customer || x.ProcessStatus == ProcessStatus.Approved).ToList();
			if ( weeklyPlans.Count == 0 )
			{
				result.StatusCode = 404;
				result.Message = "Không có weekplan phù hợp";
				return result;
			}
			if ( weeklyPlans.Count > 0 )
			{
				if ( status )
				{
					foreach ( var weeklyPlan in weeklyPlans )
					{
						weeklyPlan.BaseStatus = BaseStatus.Available;
					}
					result.StatusCode = 200;
					result.Message = "Chuyển Available thành công";
				}
				else
				{
					foreach ( var weeklyPlan in weeklyPlans )
					{
						weeklyPlan.BaseStatus = BaseStatus.UnAvailable;
					}
					result.StatusCode = 200;
					result.Message = "Chuyển UnAvailable thành công";
				}
			}
			await _unitOfWork.CompleteAsync();
			return result;
		}

	}
}
