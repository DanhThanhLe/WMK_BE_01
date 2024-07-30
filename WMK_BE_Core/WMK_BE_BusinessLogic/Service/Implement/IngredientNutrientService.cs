using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientNutrientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientNutrientModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class IngredientNutrientService : IIngredientNutrientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly CreateIngredientNutrientValidator _createIngredientNutrientValidator;
        private readonly FullIngredientNutrientValidator _fullIngredientNutrientValidator;
        public IngredientNutrientService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createIngredientNutrientValidator = new CreateIngredientNutrientValidator();
            _fullIngredientNutrientValidator = new FullIngredientNutrientValidator();
        }

        private async Task<List<IngredientNutrient>> GetAllToProcess()
        {
            var currentList = await _unitOfWork.IngredientNutrientRepository.GetAllAsync();
            if (currentList.Any())
            {
                return currentList;
            }
            return new List<IngredientNutrient>();
        }

        #region get all
        public async Task<ResponseObject<List<IngredientNutrientResponse>>> GetAll()
        {
            var result = new ResponseObject<List<IngredientNutrientResponse>>();
            List<IngredientNutrient> currentList = await GetAllToProcess();
            if (currentList.Count > 0)
            {
                result.StatusCode = 200;
                result.Message = "OK. List Ingrdient nutrient:";
                result.Data = _mapper.Map<List<IngredientNutrientResponse>>(currentList);
                return result;
            }
            result.StatusCode = 500;
            result.Message = "Not found data. Say from GetAll - IngredientNutrientService";
            return result;
        }
        #endregion

        #region get by ingredient id
        public async Task<ResponseObject<IngredientNutrientResponse>> GetByIngredientId(Guid request)
        {
            var result = new ResponseObject<IngredientNutrientResponse>();
            var currentList = await _unitOfWork.IngredientNutrientRepository.GetAllAsync();
            if (currentList.Count > 0)
            {
                IngredientNutrient foundResult = currentList.FirstOrDefault(x => x.IngredientID.ToString().Equals(request.ToString()));
                if (foundResult != null)
                {
                    result.StatusCode = 200;
                    result.Message = "OK. Ingredient nutrient with ingredient id: " + request;
                    result.Data = _mapper.Map<IngredientNutrientResponse>(foundResult);
                    return result;
                }
                else
                {
                    result.StatusCode = 400;
                    result.Message = "Not found with ingredient id: " + request + " Say from GetByIngredientId - IngredientNutrientService";
                    return result;
                }
            }
            else
            {
                result.StatusCode = 500;
                result.Message = "Empty data. Say form GetByIngredientId - IngredientNutrientService";
                return result;
            }
        }
        #endregion

        #region Create
        public async Task<ResponseObject<IngredientNutrient>> Create(Guid IngredientID, CreateIngredientNutrientRequest request)
        {
            var result = new ResponseObject<IngredientNutrient>();
            var validateResult = _createIngredientNutrientValidator.Validate(request);
            if (!validateResult.IsValid)//bat loi dau vao
            {
                var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
                result.StatusCode = 500;
                result.Message = "Say from Create - IngredientNutrientService. " + string.Join(" - /n", error);
                return result;
            }
            else//du lieu cua request ko co loi
            {
                var checkIngredient = await _unitOfWork.IngredientRepository.GetByIdAsync(IngredientID.ToString());
                var currentList = await GetAllToProcess();
                var checkExist = currentList.FirstOrDefault(x => x.IngredientID == IngredientID);
                if (checkIngredient != null && checkExist != null)//khac null nghia la co ton tai ingredient nhu tren, tim luon co thong tin nutrient chua
                {
                    result.StatusCode = 400;
                    result.Message = "Nutrient information for ingredient id " + IngredientID + " already existed. Say from Create - IngredientNutrientService";
                    return result;

                }
                else if (checkIngredient != null && checkExist == null)//nghia la co ingrerdient ton tai va chua co thong tin nutrient
                {
                    IngredientNutrient newOne = _mapper.Map<IngredientNutrient>(request);
                    newOne.IngredientID = IngredientID;
                    var createResult = await _unitOfWork.IngredientNutrientRepository.CreateAsync(newOne);
                    if (createResult)
                    {
                        await _unitOfWork.CompleteAsync();
                        //checkIngredient.IngredientNutrient = newOne;
                        result.StatusCode = 200;
                        result.Message = "OK with create nutrient information with Ingredient ID: " + IngredientID;
                        result.Data = _mapper.Map<IngredientNutrient>(newOne);
                        return result;
                    }
                    else
                    {
                        result.StatusCode = 400;
                        result.Message = "Create failed with ingredient with id " + IngredientID + ". Say from Create - IngredientNutrientService";
                        return result;
                    }

                }
                else if (checkIngredient == null)//null la coi nhu ko tim thay ingredient trong db -> bao loi khog tim thay
                {
                    result.StatusCode = 400;
                    result.Message = " Ingredient with id " + IngredientID + " not found. Say from Create - IngredientNutrientService";
                    return result;
                }
                else//cac loai ket qua khac
                {
                    result.StatusCode = 500;
                    result.Message = "Some errors may occured. Say from Create - IngredientNutrientService";
                    return result;
                }
            }
        }
        #endregion

        #region Update
        public async Task<ResponseObject<IngredientNutrientResponse>> Update(Guid id,FullIngredientNutrientRequest request)
        {
            var result = new ResponseObject<IngredientNutrientResponse>();
            try
            {
                var validateResult = _fullIngredientNutrientValidator.Validate(request);
                if (!validateResult.IsValid)
                {
                    var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
                    result.StatusCode = 500;
                    result.Message = "Say from Create - IngredientNutrientService. " + string.Join(" - ", error);
                    return result;
                }
                else
                {
                    //kiem tra voi ingredient id xem
                    //+ co ton tai ingredient ko
                    //+ co ton tai nutrient nao chua, phong bi duplicate
                    var checkIngredient = await _unitOfWork.IngredientRepository.GetByIdAsync(request.IngredientID.ToString());
                    var currentList = await _unitOfWork.IngredientNutrientRepository.GetAllAsync();
                    var checkExist = currentList.FirstOrDefault(x => x.IngredientID == request.IngredientID);
                    if (checkIngredient != null && checkExist != null && !checkExist.Id.ToString().Equals(id.ToString()))//co ingredient, co nutrient nhung khong phai cai dang update - bao loi. tranh truogn hop bi duplicate thong tin
                    {
                        result.StatusCode = 500;
                        result.Message = "Wrong id for update nutrient info with ingredient Id: " + request.IngredientID + " Say from Update - IngredientNutrientService.";
                        return result;

                    }
                    else if (checkIngredient != null && checkExist != null && checkExist.Id.ToString().Equals(id.ToString()))//co ingredient, co nutrient va la dung cai dang update
                    {
                        _unitOfWork.IngredientNutrientRepository.DetachEntity(checkExist);//detach khi can
                        _unitOfWork.IngredientRepository.DetachEntity(checkIngredient);//detach khi can
                        IngredientNutrient updateOne = _mapper.Map<IngredientNutrient>(request);
                        var updateResult = await _unitOfWork.IngredientNutrientRepository.UpdateAsync(updateOne);
                        if (updateResult)
                        {
                            await _unitOfWork.CompleteAsync();
                            result.StatusCode = 200;
                            result.Message = "OK. Success for update nutrient info with ingredient Id: " + request.IngredientID;
                            _unitOfWork.IngredientNutrientRepository.DetachEntity(updateOne);
                            return result;
                        }
                        else
                        {
                            await _unitOfWork.CompleteAsync();
                            result.StatusCode = 500;
                            result.Message = " Failed to update nutrient info with ingredient Id: " + request.IngredientID + " Say from Update - IngredientNutrientService";
                            return result;
                        }
                    }
                    else if (checkIngredient == null)//ko co ingredint
                    {
                        result.StatusCode = 500;
                        result.Message = "Not found ingredient with Id: " + request.IngredientID + " Say from Update - IngredientNutrientService";
                        return result;
                    }
                    else if (checkExist == null)//ko co nutrient info tuong ung
                    {
                        result.StatusCode = 500;
                        result.Message = "not found nutrient info with ingredient Id: " + request.IngredientID + " Say from Update - IngredientNutrientService";
                        return result;
                    }
                    else//cac loi khac
                    {
                        result.StatusCode = 500;
                        result.Message = "Some error may occured. Say from Update - IngredientNutrientService";
                        return result;
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

        #region delete
        public async Task<ResponseObject<IngredientNutrientResponse>> DeleteById(Guid request)
        {
            var result = new ResponseObject<IngredientNutrientResponse>();
            var checkExist = await _unitOfWork.IngredientNutrientRepository.GetByIdAsync(request.ToString());
            if (checkExist == null)
            {
                result.StatusCode = 500;
                result.Message = "Not found. Say from DeleteIngredientById - IngredientService";
                return result;
            }
            else
            {
                _unitOfWork.IngredientNutrientRepository.DetachEntity(checkExist);
                var deleteResult = await _unitOfWork.IngredientNutrientRepository.DeleteAsync(request.ToString());
                if (deleteResult)
                {
                    await _unitOfWork.CompleteAsync();
                    result.StatusCode = 200;
                    result.Message = "Success";
                    return result;
                }
                else
                {
                    result.StatusCode = 500;
                    result.Message = "Error at delete nutrient info with id " + request + ". Say from DeleteIngredientById - IngredientService";
                    return result;
                }
            }
        }
        #endregion

        #region get by id
        public async Task<ResponseObject<IngredientNutrientResponse>> GetById(Guid request)
        {
            var result = new ResponseObject<IngredientNutrientResponse>();
            var foundResult = await _unitOfWork.IngredientNutrientRepository.GetByIdAsync(request.ToString());
            if (foundResult != null)
            {
                result.StatusCode = 200;
                result.Message = "OK. Ingredient nutrient info";
                result.Data = _mapper.Map<IngredientNutrientResponse>(foundResult);
                return result;
            }
            else
            {
                result.StatusCode = 400;
                result.Message = "Not found with ingredient id: " + request + " Say from GetByIngredientId - IngredientNutrientService";
                return result;
            }
        }
        #endregion
    }
}

