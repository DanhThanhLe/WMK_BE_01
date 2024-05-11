using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class IngredientService : IIngredientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IngredientValidator _validator;
        private readonly UpdateIngredientValidator _updateValidator;
        private readonly UpdateStatusIngredientValidator _updateStatusValidator;
        private readonly IdValidator _idValidator;
        public IngredientService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = new IngredientValidator();
            _updateValidator = new UpdateIngredientValidator();
            _updateStatusValidator = new UpdateStatusIngredientValidator();
            _idValidator = new IdValidator();
        }
        #region Change status
        public async Task<ResponseObject<IngredientResponse>> ChangeStatus(UpdateStatusIngredientrequest ingredient)
        {
            var result = new ResponseObject<IngredientResponse>();
            var validateResult = _updateStatusValidator.Validate(ingredient);
            if (!validateResult.IsValid)
            {
                var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
                result.StatusCode = 400;
                result.Message = string.Join(" - ", error);
                return result;
            }
            var found = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredient.Id.ToString());
            if (found != null)
            {
                result.StatusCode = 400;
                result.Message = "Not found ingredient";
                return result;
            }
            var changeResult = await _unitOfWork.IngredientRepository.ChangeStatusAsync(ingredient.Id, ingredient.Status);
            if (changeResult)
            {
                await _unitOfWork.CompleteAsync();
                result.StatusCode = 200;
                result.Message = "Change Ingredient " + ingredient.Id + " status Successfully";
                return result;
            }
            else
            {
                result.StatusCode = 400;
                result.Message = "Change Ingredient " + ingredient.Id + " status Unsuccessfully";
                return result;
            }
        }
        #endregion
        #region Create
        public async Task<ResponseObject<IngredientResponse>> CreateIngredient(CreateIngredientRequest ingredient)
        {
            var result = new ResponseObject<IngredientResponse>();
            var currentList = await _unitOfWork.IngredientRepository.GetAllAsync();
            var validateResult = _validator.Validate(ingredient);
            if (!validateResult.IsValid)
            {
                var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
                result.StatusCode = 500;
                result.Message = string.Join(" - ", error);
                return result;
            }
            var found = currentList.FirstOrDefault(i => i.Name == ingredient.Name);
            if (found != null && found.Status.ToString().Equals("Available"))
            {
                result.StatusCode = 500;
                result.Message = "Existed with ID: " + found.Id;
                return result;
            }
            string emptyData = "Empty";
            if (ingredient.Img == null)
            {
                ingredient.Img = emptyData;
            }
            if (ingredient.UpdatedAt == null)
            {
                ingredient.UpdatedAt = DateTime.UtcNow;
            }
            if (ingredient.UpdatedBy == null)
            {
                ingredient.UpdatedBy = ingredient.CreatedBy;
            }
            Ingredient newIngredient = _mapper.Map<Ingredient>(ingredient);
            var createResult = await _unitOfWork.IngredientRepository.CreateAsync(newIngredient);
            if (createResult)
            {
                result.StatusCode = 200;
                result.Message = "Create successfully"; //kiem tra voi cach khac, dung ham Create1, sau do cho tim ingredient voi name vua tao, nau tim thay thi lay Id tra ve, ko tim thay nghia la loi --> bao loi
                return result;
            }
            else
            {
                result.StatusCode = 500;
                result.Message = "Error at create";
                return result;
            }
        }
        #endregion
        #region Delete from Database
        public async Task<ResponseObject<IngredientResponse>> DeleteIngredientById(IdIngredientRequest ingredient)//ko khuyen khich dung
        {
            var result = new ResponseObject<IngredientResponse>();
            var validateResult = _idValidator.Validate(ingredient);
            if (validateResult != null)
            {
                var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
                result.StatusCode = 400;
                result.Message = string.Join(" - ", error);
                return result;
            }
            var found = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredient.Id.ToString());
            if (found == null)
            {
                result.StatusCode = 500;
                result.Message = "not found";
                return result;
            }
            else
            {
                var deleteResult = await _unitOfWork.IngredientRepository.DeleteAsync(ingredient.Id.ToString());
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
                    result.Message = "Error at delete";
                    return result;
                }
            }
        }
        #endregion
        #region Get by ID
        public async Task<ResponseObject<IngredientResponse>> GetIngredientById(string id)
        {
            var result = new ResponseObject<IngredientResponse>();
            var ingredients = await _unitOfWork.IngredientRepository.GetByIdAsync(id);
            if (ingredients != null)
            {
                result.StatusCode = 200;
                result.Message = "OK. Ingredients with id " + id;
                IngredientResponse response = _mapper.Map<IngredientResponse>(ingredients);
                result.Data = response;
                return result;
            }
            else
            {
                result.StatusCode = 404;
                result.Message = "Not found. Data not found or wrong id";
                return result;
            }
        }
        #endregion
        #region Get by name
        public async Task<ResponseObject<IngredientResponse>> GetIngredientByName(string name)
        {
            var result = new ResponseObject<IngredientResponse>();
            var ingredientList = await _unitOfWork.IngredientRepository.GetAllAsync();
            if (ingredientList != null && ingredientList.Count() > 0)
            {
                //var foundList = ingredientList.Where(x => x.Name.Contains(name)).ToList();
                var foundList = ingredientList.Where(x => x.Name.StartsWith(name)).ToList();
                if (foundList == null)
                {
                    result.StatusCode = 404;
                    result.Message = "Not found. No such ingredient in collection contain keyword: " + name;
                    return result;

                }
                else
                {
                    result.StatusCode = 200;
                    result.Message = "Ingredient list found by name";
                    result.List = _mapper.Map<List<IngredientResponse>>(foundList);
                }
            }
            else
            {
                result.StatusCode = 404;
                result.Message = "Not found. Empty list or Data not found";
                return result;
            }
            return result;
        }
        #endregion
        #region Get all
        public async Task<ResponseObject<IngredientResponse>> GetIngredients()
        {
            var result = new ResponseObject<IngredientResponse>();
            var ingredients = await _unitOfWork.IngredientRepository.GetAllAsync();
            var responseList = ingredients.ToList().Where(x => x.Status == 0);
            if (ingredients != null && ingredients.Count > 0)
            {
                result.StatusCode = 200;
                result.Message = "OK. Ingredients list";
                result.List = _mapper.Map<List<IngredientResponse>>(responseList);
                return result;
            }
            else
            {
                result.StatusCode = 404;
                result.Message = "Not found. Empty list or Data not found";
                return result;
            }
        }
        #endregion
        #region Remove from app
        public async Task<ResponseObject<IngredientResponse>> RemoveIngredientById(IdIngredientRequest ingredient)
        {
            var result = new ResponseObject<IngredientResponse>();
            var validateResult = _idValidator.Validate(ingredient);
            if (!validateResult.IsValid)
            {
                var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
                result.StatusCode = 400;
                result.Message = string.Join(" - ", error);
                return result;
            }
            var found = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredient.Id.ToString());
            if (found != null)
            {
                result.StatusCode = 400;
                result.Message = "Not found ingredient";
                return result;
            }
            else
            {
                found.Status = BaseStatus.Unavailable;
                var removeResult = await _unitOfWork.IngredientRepository.UpdateAsync(found);
                if (removeResult)
                {
                    await _unitOfWork.CompleteAsync();
                    result.StatusCode = 200;
                    result.Message = "Remove Success";
                    return result;
                }
                else
                {
                    result.StatusCode = 500;
                    result.Message = "Error at remvove ingredient";
                    return result;
                }
            }
        }
        #endregion
        #region Update 
        public async Task<ResponseObject<IngredientResponse>> UpdateIngredient(IngredientRequest ingredient)
        {
            var result = new ResponseObject<IngredientResponse>();
            var validateResult = _updateValidator.Validate(ingredient);
            var currentList = await _unitOfWork.IngredientRepository.GetAllAsync();
            if (validateResult != null)//validate
            {
                var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
                result.StatusCode = 400;
                result.Message = string.Join(" - ", error);
                return result;
            }
            var foundUpdate = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredient.Id.ToString());
            if (foundUpdate == null)//check exist
            {
                result.StatusCode = 500;
                result.Message = "Not found with ID: " + ingredient.Id;
                return result;
            }
            else//bat dau update
            {
                var duplicateName = currentList.FirstOrDefault(i => i.Name == ingredient.Name);//check trung ten voi ingrdient available
                if (duplicateName != null && duplicateName.Status.ToString().Equals("Available"))
                {
                    result.StatusCode = 500;
                    result.Message = "Existed with ID: " + duplicateName.Id;
                    return result;
                }
                else
                {
                    foundUpdate.Category = ingredient.Category;
                    foundUpdate.Name = ingredient.Name;
                    foundUpdate.Img = ingredient.Img;
                    foundUpdate.PricebyUnit = ingredient.PricebyUnit;
                    foundUpdate.Unit = ingredient.Unit;
                    foundUpdate.Status = ingredient.Status;
                    foundUpdate.CreatedAt = ingredient.CreatedAt;
                    foundUpdate.CreatedBy = ingredient.CreatedBy;
                    foundUpdate.UpdatedAt = ingredient.UpdatedAt;
                    foundUpdate.UpdatedBy = ingredient.UpdatedBy;
                    var updateResult = await _unitOfWork.IngredientRepository.UpdateAsync(foundUpdate);
                    if (updateResult)
                    {
                        await _unitOfWork.CompleteAsync();
                        result.StatusCode = 200;
                        result.Message = "Update done";
                        return result;
                    }
                    else
                    {
                        result.StatusCode = 500;
                        result.Message = "Update error";
                        return result;
                    }
                }
            }
        }
        #endregion
    }
}
