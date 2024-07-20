using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.WeeklyPlanModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class WeeklyPlanService : IWeeklyPlanService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IRecipePlanService _recipePlanService;
		private readonly IMapper _mapper;

		#region Validator
		private readonly CreateWeeklyPlanValidator _createValidator;
		private readonly UpdateWeeklyPlanValidator _updateValidator;
		private readonly DeleteWeeklyPlanValidator _deleteValidator;
		private readonly ChangeStatusWeeklyPlanValidator _changeStatusValidator;
		#endregion
		public WeeklyPlanService(IUnitOfWork unitOfWork, IMapper mapper, IRecipePlanService recipePlanService)
		{
			_unitOfWork = unitOfWork;
			_recipePlanService = recipePlanService;
			_mapper = mapper;

			_createValidator = new CreateWeeklyPlanValidator();
			_updateValidator = new UpdateWeeklyPlanValidator();
			_deleteValidator = new DeleteWeeklyPlanValidator();
			_changeStatusValidator = new ChangeStatusWeeklyPlanValidator();
		}


        #region get all

		public async Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetAllAsync()
		{
			var result = new ResponseObject<List<WeeklyPlanResponseModel>>();
			var weeklyPlans = await _unitOfWork.WeeklyPlanRepository.GetAllAsync();
			if (weeklyPlans != null && weeklyPlans.Count > 0)
			{
				var returnLists = _mapper.Map<List<WeeklyPlanResponseModel>>(weeklyPlans);
				//foreach (var weeklyPlan in returnLists)
				//{
				//	var recipeList = new List<RecipeResponse>();
				//	foreach (var recipe in weeklyPlan.Recipes)
				//	{
				//		recipeList.Add(recipe);
				//	}
				//	weeklyPlan.Recipes = recipeList;
				//}
				result.StatusCode = 200;
				result.Message = "WeeklyPlan list: "+returnLists.Count();
				result.Data = returnLists;
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
			if (weeklyPlanExist != null && weeklyPlanExist.ProcessStatus == ProcessStatus.Approved)
			{
				var weeklyPlan = _mapper.Map<WeeklyPlanResponseModel>(weeklyPlanExist);
				result.StatusCode = 200;
				result.Message = "Weekly plan";
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

		#region Create

		public async Task<ResponseObject<WeeklyPlanResponseModel>> CreateWeeklyPlanAsync(CreateWeeklyPlanRequest model)
		{
			var result = new ResponseObject<WeeklyPlanResponseModel>();
			try
			{
				var validateResult = _createValidator.Validate(model);
				if (!validateResult.IsValid)
				{
					var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
					result.StatusCode = 400;
					result.Message = string.Join(" - ", error);
					return result;
				}
				//check user exist 
				var userExist = await _unitOfWork.UserRepository.GetByIdAsync(model.CreatedBy);
				if (userExist == null || userExist.Role != Role.Staff)
				{
					result.StatusCode = 404;
					result.Message = "User not exist or not have access!";
					return result;
				}
				List<WeeklyPlan> currentList = await _unitOfWork.WeeklyPlanRepository.GetAllAsync();
				if (currentList.Count() > 0)
				{
					WeeklyPlan foundDuplicate = currentList.Where(x => x.Description.Trim().Equals(model.Description.Trim())).SingleOrDefault();
					if (foundDuplicate != null && (foundDuplicate.ProcessStatus == ProcessStatus.Processing || foundDuplicate.ProcessStatus == ProcessStatus.Approved))
					{
                        result.StatusCode = 400;
						result.Message = "Weekly plan Description already existed";
                        return result;
                    }
				}
				//create weekly plan
				var newWeeklyPlan = _mapper.Map<WeeklyPlan>(model);
				newWeeklyPlan.CreateAt = DateTime.Now;
				int countMeal = 0;
				//check size of recipe create by staff
				foreach (var item in model.recipeIds)
				{
					countMeal += item.Quantity;
				}
				if (countMeal < 21 || countMeal > 200)//( model.recipeIds.Count < 5 || model.recipeIds.Count > 21 )
				{
					result.StatusCode = 402;
					result.Message = "Must be 21 protion for each week " + countMeal;//"Recipe must be 5 - 21!";
					return result;
				}
				newWeeklyPlan.EndDate = DateTime.Now.AddDays(2);//add endDate after 2 day if staff create

				//add new weekly plan
				var createResult = await _unitOfWork.WeeklyPlanRepository.CreateAsync(newWeeklyPlan);
				if (!createResult)
				{
					result.StatusCode = 500;
					result.Message = "Create weekly plan unsuccessfully!";
					return result;
				}

				await _unitOfWork.CompleteAsync();//Save database

				//create list recipePlan
				var createRecipePlansResult = await _recipePlanService.CreateRecipePlanAsync(newWeeklyPlan.Id, model.recipeIds);
				if (createRecipePlansResult.StatusCode == 200 && createRecipePlansResult.Data != null)
				{
					//assign the value of recipe plan to new weekly plan
					newWeeklyPlan.RecipePLans = createRecipePlansResult.Data;
					await _unitOfWork.WeeklyPlanRepository.UpdateAsync(newWeeklyPlan);
					await _unitOfWork.CompleteAsync();
					result.StatusCode = createRecipePlansResult.StatusCode;
					result.Message = "Create Weekly plan successfully.";
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
			catch (Exception ex)
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
				return result;
			}
		}


        #endregion

        #region update

        public async Task<ResponseObject<WeeklyPlanResponseModel>> UpdateWeeklyPlanAsync(UpdateWeeklyPlanRequestModel model)
		{
			var result = new ResponseObject<WeeklyPlanResponseModel>();
			try
			{
				var validateResult = _updateValidator.Validate(model);
				if (!validateResult.IsValid)
				{
					var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
					result.StatusCode = 400;
					result.Message = string.Join(" - ", error);
					return result;
				}
				//check weekly plan exist
				var weeklyPlanExist = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(model.Id.ToString());
				if (weeklyPlanExist == null)
				{
					result.StatusCode = 404;
					result.Message = "Weekly plan not exist!";
					return result;
				}
				//if status is approve or cancel cant update
				switch (weeklyPlanExist.ProcessStatus)
				{
					case ProcessStatus.Approved:
						result.StatusCode = 402;
						result.Message = "Can't update this weekly plan with approve!";
						return result;
					case ProcessStatus.Cancel:
						result.StatusCode = 402;
						result.Message = "Can't update this weekly plan with Cancel!";
						return result;
					default:
						break;
				}
				var weeklyPlanUpdate = _mapper.Map<WeeklyPlan>(model);
				var updateRecipePlansResult = await _recipePlanService.UpdateRecipePlanAsync(model.Id, model.recipeIds);
				if (updateRecipePlansResult.StatusCode == 200 && updateRecipePlansResult.Data != null)
				{
					weeklyPlanUpdate.RecipePLans = updateRecipePlansResult.Data;
					await _unitOfWork.WeeklyPlanRepository.UpdateAsync(weeklyPlanUpdate);
					await _unitOfWork.CompleteAsync();
					result.StatusCode = updateRecipePlansResult.StatusCode;
					result.Message = "Update Weekly plan successfully.";
					return result;
				}
				else
				{
					result.StatusCode = updateRecipePlansResult.StatusCode;
					result.Message = updateRecipePlansResult.Message;
					return result;
				}
			}
			catch (Exception ex)
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
				return result;
			}
		}


        #endregion 

        #region Delete
        public async Task<ResponseObject<WeeklyPlanResponseModel>> DeleteWeeklyPlanAsync(DeleteWeeklyPlanRequestModel model)
		{
			var result = new ResponseObject<WeeklyPlanResponseModel>();
			var validateResult = _deleteValidator.Validate(model);
			if (!validateResult.IsValid)
			{
				var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - ", error);
				return result;
			}

			var weeklyPlanExist = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(model.Id.ToString());
			if (weeklyPlanExist == null)
			{
				result.StatusCode = 404;
				result.Message = "Weekly plan not exist!";
				return result;
			}
			//check recipe have in weekly plan
			var RecipeExist = await _unitOfWork.WeeklyPlanRepository.RecipeExistInWeeklyPlanAsync(model.Id);
			if (RecipeExist)
			{
				//if have just change status
				weeklyPlanExist.ProcessStatus = ProcessStatus.Cancel;
				var changeResult = await _unitOfWork.WeeklyPlanRepository.UpdateAsync(weeklyPlanExist);
				if (changeResult)
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Change weekly plan status with id (" + weeklyPlanExist.Id + ") successfullly";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Change weekly plan status with id (" + weeklyPlanExist.Id + ") unsuccessfullly!";
					return result;
				}
			}
			else
			{
				var deleteResult = await _unitOfWork.WeeklyPlanRepository.DeleteAsync(weeklyPlanExist.Id.ToString());
				if (deleteResult)
				{
					result.StatusCode = 200;
					result.Message = "Delete weekly plan with id (" + weeklyPlanExist.Id + ") successfullly";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Delete weekly plan with id (" + weeklyPlanExist.Id + ") unsuccessfullly!";
					return result;
				}
			}
		}

        #endregion

        #region Change status
        public async Task<ResponseObject<WeeklyPlanResponseModel>> ChangeStatusWeeklyPlanAsync(ChangeStatusWeeklyPlanRequestModel model)
		{
			var result = new ResponseObject<WeeklyPlanResponseModel>();
			var validateResult = _changeStatusValidator.Validate(model);
			if (!validateResult.IsValid)
			{
				var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - ", error);
				return result;
			}

			var weeklyPlanExist = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(model.Id.ToString());
			if (weeklyPlanExist == null)
			{
				result.StatusCode = 404;
				result.Message = "Weekly plan not exist!";
				return result;
			}

			weeklyPlanExist.ProcessStatus = model.ProcessStatus;
			var changeResult = await _unitOfWork.WeeklyPlanRepository.UpdateAsync(weeklyPlanExist);
			if (changeResult)
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Change status (" + weeklyPlanExist.ProcessStatus + ") successfully";
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Change status (" + weeklyPlanExist.ProcessStatus + ") unsuccessfully!";
				return result;
			}
		}

		#endregion
	} 
}
