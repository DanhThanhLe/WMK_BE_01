using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
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
        public WeeklyPlanService(IUnitOfWork unitOfWork, IMapper mapper, IRecipePlanService recipePlanService, IRedisService redisService)
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

        private async Task<List<WeeklyPlan>> GetAllToProcess()
        {
            var currentList = await _unitOfWork.WeeklyPlanRepository.GetAllAsync();
            if (currentList.Any())
            {
                return currentList;
            }
            return new List<WeeklyPlan>();
        }

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
                    WeeklyPlan foundDuplicate = currentList.Where(x => x.Description.Trim().Equals(model.Description.Trim())).FirstOrDefault();
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

        #region create for customer
        public async Task<ResponseObject<WeeklyPlanResponseModel>> CreateForSutomer(CreateWeeklyPlanRequest request)
        {   //kiem tra processStatus -> khong dung cho customer -> bao loi
            //kiem tra nguoi dung -> ko dung customer -> bao loi
            //kiem tra so luong weekPlan ma customer dang xet hien co -> du 5 thi bao loi. chua du 5 thi bat dau tao moi
            //so luong recipe vuot qua 200 thi bao loi ko cho tao qua 200
            //kiem tra thong tin cac mon an (recipeId) status la ko phai approved thi bao loi
            //bat dau tao recipePlan -> dung nhu ham Create ben tren. ke qua nhan ve loi thi xoa weekPlan vua tao va bao loi

            var result = new ResponseObject<WeeklyPlanResponseModel>();
            try//bat moi loi bat ngo va chu co message cu the 
            {
                if (request.ProcessStatus != null && request.ProcessStatus != ProcessStatus.Customer)//kiem tra processStatus
                {
                    result.StatusCode = 500;
                    result.Message = "ProcessStatus not for customer";
                    return result;
                }
                var userExist = await _unitOfWork.UserRepository.GetByIdAsync(request.CreatedBy);//kiem tra role cua nguoi tao (createBy), role phai la customer
                if (userExist == null || userExist.Role != Role.Customer)
                {
                    result.StatusCode = 404;
                    result.Message = "User not exist or not have access!";
                    return result;
                }
                List<WeeklyPlan> currentList = await _unitOfWork.WeeklyPlanRepository.GetAllAsync();
                if (currentList.Count() > 0)
                {
                    WeeklyPlan foundDuplicate = currentList.Where(x => x.Description.Trim().Equals(request.Title.Trim())).FirstOrDefault();
                    if (foundDuplicate != null && foundDuplicate.ProcessStatus == ProcessStatus.Customer)
                    {
                        result.StatusCode = 400;
                        result.Message = "Weekly plan Title already existed";
                        return result;
                    }
                    else
                    {
                        int countPlan = 0; //tinh xem dang co bao nhieu plan duoc tao boi nguoi dung roi. qua 5 thi ko cho tao them
                        foreach (var item in currentList)
                        {
                            if (item.CreatedBy == request.CreatedBy)
                            {
                                countPlan++;
                            }
                        }
                        if (countPlan >= 5)
                        {
                            result.StatusCode = 400;
                            result.Message = "Reach limit in max personal plan. Remove some personal plan before create new. ";
                            return result;
                        }
                        WeeklyPlan newOne = _mapper.Map<WeeklyPlan>(request);
                        newOne.CreateAt = DateTime.Now;
                        var createResult = await _unitOfWork.WeeklyPlanRepository.CreateAsync(newOne);
                        if (createResult)
                        {
                            await _unitOfWork.CompleteAsync();
                            var createRecipePlanResult = await _recipePlanService.CreateRecipePlanAsync(newOne.Id, request.recipeIds);
                            if (createRecipePlanResult.StatusCode == 200 && createRecipePlanResult.Data != null)
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
            catch (Exception ex)
            {
                result.StatusCode = 500;
                result.Message = ex.Message;
                return result;
            }
        }
        #endregion

        #region get all

        public async Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetAllAsync()
        {
            var result = new ResponseObject<List<WeeklyPlanResponseModel>>();

            //get from redis
            var redisKey = "WeeklyPlanList";
            var redisData = await _redisService.GetValueAsync<List<WeeklyPlanResponseModel>>(redisKey);
            if (redisData != null && redisData.Count > 0)
            {
                result.StatusCode = 200;
                result.Message = "WeeklyPlan list: " + redisData.Count();
                result.Data = redisData;
                return result;
            }

            var weeklyPlans = await _unitOfWork.WeeklyPlanRepository.Get(x => x.ProcessStatus == ProcessStatus.Approved).ToListAsync();
            var returnList = weeklyPlans.Where(x => x.ProcessStatus == ProcessStatus.Approved).ToList();
            if (weeklyPlans != null && weeklyPlans.Count > 0)
            {
                var returnResult = _mapper.Map<List<WeeklyPlanResponseModel>>(returnList);
                result.StatusCode = 200;
                result.Message = "WeeklyPlan list: " + returnResult.Count();
                result.Data = returnResult;

                //set cache to redis
                await _redisService.SetValueAsync(redisKey, returnResult, TimeSpan.FromDays(3));

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

        #region get week plan list with user id
        public async Task<ResponseObject<List<WeeklyPlanResponseModel>>> GetListByCustomerId(Guid customerId)
        {
            var result = new ResponseObject<List<WeeklyPlanResponseModel>>();
            try
            {
                //var currentWeekPlanList = await GetAllToProcess();
                //if (currentWeekPlanList.Count() == 0)
                //{
                //    result.StatusCode = 500;
                //    result.Message = "Empty list week plan";
                //    return result;
                //}
                //else
                //{
                ////var foundList = currentWeekPlanList.Where(x => x.CreatedBy.ToLower().Equals(customerId.ToString().ToLower())).ToList();
                var foundList = _unitOfWork.WeeklyPlanRepository.Get(x => x.CreatedBy.ToLower().Equals(customerId.ToString().ToLower())).ToList();
                if (foundList.Count() == 0) //ko tim dc
                {
                    result.StatusCode = 500;
                    result.Message = "Not found with user id";
                    return result;
                }
                else //co thong tin
                {
                    var returnList = _mapper.Map<List<WeeklyPlanResponseModel>>(foundList);
                    result.StatusCode = 200;
                    result.Message = "Ok";
                    result.Data = returnList;
                    return result;
                }
                //}
            }
            catch (Exception ex)
            {
                result.StatusCode = 500;
                result.Message = ex.Message;
                return result;
            }
        }
        #endregion

        #region update - full info
        public async Task<ResponseObject<WeeklyPlanResponseModel>> UpdateFullInfo(UpdateWeeklyPlanRequest request)
        {
            var result = new ResponseObject<WeeklyPlanResponseModel>();
            try
            {
                //tim thong tin cua weeklyPlan - ok
                //cap nhat thong tin co ban cho week plan do
                //tim thong tin cho tat ra recipePlan lien quan - ok
                //xoa het thong tin - ok
                //tao lai thong tin moi - ok
                if (request.recipeIds.Count > 200)
                {
                    result.Message = "Vuot qua pham vi cho phep. dat toi da duoi 200 con thuc";
                    return result;
                }
                else
                {
                    var foundWeeklyPlan = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(request.Id.ToString());
                    if (foundWeeklyPlan == null)
                    {
                        result.Message = "Not found week plan";
                        return result;
                    }
                    else//bat dau tim recipePlans
                    {
                        //bat dau thay doi thong tin cho week plan
                        _unitOfWork.WeeklyPlanRepository.DetachEntity(foundWeeklyPlan);
                        foundWeeklyPlan.BeginDate = request.BeginDate != null ? request.BeginDate : foundWeeklyPlan.BeginDate;
                        foundWeeklyPlan.EndDate = request.EndDate != null ? request.EndDate : foundWeeklyPlan.EndDate;
                        foundWeeklyPlan.Description = request.Description != null ? request.Description : foundWeeklyPlan.Description;
                        foundWeeklyPlan.UrlImage = request.UrlImage != null ? request.UrlImage : foundWeeklyPlan.UrlImage;
                        foundWeeklyPlan.Title = request.Title != null ? request.Title : foundWeeklyPlan.Title;
                        foundWeeklyPlan.Notice = request.Notice != null ? request.Notice : foundWeeklyPlan.Notice;
                        foundWeeklyPlan.ApprovedAt = request.ApprovedAt != null ? request.ApprovedAt : foundWeeklyPlan.ApprovedAt;
                        foundWeeklyPlan.ApprovedBy = request.ApprovedBy != null ? request.ApprovedBy : foundWeeklyPlan.ApprovedBy;
                        foundWeeklyPlan.UpdatedAt = request.UpdatedAt != null ? request.UpdatedAt : foundWeeklyPlan.UpdatedAt;
                        foundWeeklyPlan.UpdatedBy = request.UpdatedBy != null ? request.UpdatedBy : foundWeeklyPlan.UpdatedBy;
                        foundWeeklyPlan.ProcessStatus = (ProcessStatus)request.ProcessStatus != null ? (ProcessStatus)request.ProcessStatus : foundWeeklyPlan.ProcessStatus;

                        var updateWeeklyPlanResult = await _unitOfWork.WeeklyPlanRepository.UpdateAsync(foundWeeklyPlan);
                        if (updateWeeklyPlanResult) //update thanh cong
                        {
                            var relatedRecipePlans = _unitOfWork.RecipePlanRepository.Get(x => x.StandardWeeklyPlanId.ToString().ToLower().Equals(request.Id.ToString().ToLower())).ToList();
                            if (relatedRecipePlans.Any())
                            {
                                foreach (var item in relatedRecipePlans.ToList())
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
                            var createRecipePlansResult = await _recipePlanService.CreateRecipePlanAsync(request.Id, request.recipeIds);
                            if (createRecipePlansResult.StatusCode == 200 && createRecipePlansResult.Data != null)
                            {

                                await _unitOfWork.CompleteAsync(); //sau khi xac dinh da tao duoc ban cap nhat roi thi xoa di ban cap nhat cu 
                                result.StatusCode = createRecipePlansResult.StatusCode;
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
    }
}
