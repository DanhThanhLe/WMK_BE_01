using AutoMapper;
using FluentValidation;
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
        private readonly IIngredientNutrientService _ingredientNutrientService;
        public IngredientService(IMapper mapper, IUnitOfWork unitOfWork, IIngredientNutrientService ingredientNutrientService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = new IngredientValidator();
            _updateValidator = new UpdateIngredientValidator();
            _updateStatusValidator = new UpdateStatusIngredientValidator();
            _ingredientNutrientService = ingredientNutrientService;
        }

        #region Change status
        public async Task<ResponseObject<IngredientResponse>> ChangeStatus(UpdateStatusIngredientRequest ingredient)
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
            if (found == null)
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
            var checkIngredientCategory = await _unitOfWork.IngredientCategoryRepository.GetByIdAsync(ingredient.IngredientCategoryId.ToString());
            if (checkIngredientCategory == null)
            {
                result.StatusCode = 400;
                result.Message = "Ingredient category with id: " + ingredient.IngredientCategoryId + " not exist";
                return result;
            }
            var found = currentList.FirstOrDefault(i => i.Name == ingredient.Name);
            if (found != null)
            {
                result.StatusCode = 400;
                result.Message = "Existed with ID: " + found.Id;
                return result;
            }
            Ingredient newIngredient = _mapper.Map<Ingredient>(ingredient);
            newIngredient.CreatedAt = DateTime.UtcNow;
            newIngredient.UpdatedAt = DateTime.UtcNow;
            newIngredient.UpdatedBy = ingredient.CreatedBy;
            newIngredient.IngredientCategory = checkIngredientCategory;

            var createResult = await _unitOfWork.IngredientRepository.CreateAsync(newIngredient);
            if (createResult)
            {
                await _unitOfWork.CompleteAsync();
                //bat dau tao IngredientNutrient
                var createIngredientNutrient = await _ingredientNutrientService.Create(newIngredient.Id, ingredient.NutrientInfo);
                if (createIngredientNutrient.StatusCode == 200 && createIngredientNutrient.Data != null)
                {
                    //await _unitOfWork.CompleteAsync(); //ko can cai nay
                    result.StatusCode = 200;
                    result.Message = "Create successfully";
                    return result;
                }
                else
                {
                    await _unitOfWork.IngredientRepository.DeleteAsync(newIngredient.Id.ToString());
                    await _unitOfWork.CompleteAsync();
                    result.StatusCode = 500;
                    result.Message = "Error at create. Say from CreateIngredient - IngredientService";
                    return result;
                }
            }
            else
            {
                result.StatusCode = 500;
                result.Message = "Error at create. Say from CreateIngredient - IngredientService";
                return result;
            }
        }
        #endregion

        #region Delete from Database
        public async Task<ResponseObject<IngredientResponse>> DeleteIngredientById(Guid id)//ko khuyen khich dung
        {
            var result = new ResponseObject<IngredientResponse>();
            var found = await _unitOfWork.IngredientRepository.GetByIdAsync(id.ToString());
            if (found == null)
            {
                result.StatusCode = 500;
                result.Message = "Not found. Say from DeleteIngredientById - IngredientService";
                return result;
            }
            else
            {
                var deleteResult = await _unitOfWork.IngredientRepository.DeleteAsync(id.ToString());
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
                    result.Message = "Error at delete ingredient with id " + id + ". Say from DeleteIngredientById - IngredientService";
                    return result;
                }
            }
        }
        #endregion

        #region Get by ID
        public async Task<ResponseObject<IngredientResponse>> GetIngredientById(Guid id)
        {
            var result = new ResponseObject<IngredientResponse>();
            var ingredients = await _unitOfWork.IngredientRepository.GetByIdAsync(id.ToString());
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
                var foundList = ingredientList.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
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
        public async Task<ResponseObject<List<IngredientResponse>>> GetIngredients()
        {
            var result = new ResponseObject<List<IngredientResponse>>();
            var ingredients = await _unitOfWork.IngredientRepository.GetAllAsync();
            var responseList = ingredients.ToList().Where(x => x.Status == BaseStatus.Available);
            if (responseList != null && responseList.Count() > 0)
            {
                result.StatusCode = 200;
                result.Message = "OK. Ingredients list";
                result.Data = _mapper.Map<List<IngredientResponse>>(responseList);
                #region old
                //foreach ( var ingredientResponse in result.List )
                //{
                //	var ingredient = responseList.FirstOrDefault(i => i.Id == ingredientResponse.Id);
                //	if ( ingredient != null )
                //	{
                //		ingredientResponse.IngredientNutrient = _mapper.Map<IngredientNutrientResponse>(ingredient.IngredientNutrient);
                //                    ingredientResponse.IngredientCategory = _mapper.Map<IngredientCategoryResponse>(ingredient.IngredientCategory);
                //                }
                //}
                #endregion
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
        public async Task<ResponseObject<IngredientResponse>> RemoveIngredientById(Guid id)
        {
            var result = new ResponseObject<IngredientResponse>();
            if (id.ToString() == null)
            {
                result.StatusCode = 400;
                result.Message = "Empty request. ingredientId is empty";
                return result;
            }
            var found = await _unitOfWork.IngredientRepository.GetByIdAsync(id.ToString());
            if (found == null)
            {
                result.StatusCode = 400;
                result.Message = "Not found ingredient";
                return result;
            }
            else
            {
                found.Status = BaseStatus.UnAvailable;
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
            if (!validateResult.IsValid)//validate
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
                if (duplicateName != null && duplicateName.Status.ToString().Equals("Available") && !duplicateName.Id.Equals(ingredient.Id))
                {
                    result.StatusCode = 500;
                    result.Message = "Name existed with ID: " + duplicateName.Id;
                    return result;
                }
                else
                {
                    //foundUpdate.Category = ingredient.Category;
                    foundUpdate.Name = ingredient.Name;
                    foundUpdate.Img = ingredient.Img;
                    foundUpdate.Unit = ingredient.Unit;
                    foundUpdate.Status = ingredient.Status;
                    foundUpdate.UpdatedAt = ingredient.UpdatedAt;
                    foundUpdate.UpdatedBy = ingredient.UpdatedBy;
                    foundUpdate.Price = ingredient.Price;
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
