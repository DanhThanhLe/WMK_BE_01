using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class RecipeService : IRecipeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RecipeValidator _validator;
        private readonly IMapper _mapper;
        private readonly RecipeChangeStatusValidator _recipeChangeStatusValidator;
        private readonly IdRecipeValidator _idValidator;
        public RecipeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = new RecipeValidator();
            _recipeChangeStatusValidator = new RecipeChangeStatusValidator();
            _idValidator = new IdRecipeValidator();
        }


        #region Get all
        public async Task<ResponseObject<RecipeResponse>> GetRecipes()
        {
            var result = new ResponseObject<RecipeResponse>();
            var recipes = await _unitOfWork.RecipeRepository.GetAllAsync();
            var responseList = recipes.ToList().Where(x => x.ProcessStatus.ToInt() == 1);//??? sao qua day can cai nay?
            if (responseList != null && responseList.Count() > 0)
            {
                result.StatusCode = 200;
                result.Message = "OK. Ingredients list";
                result.List = _mapper.Map<List<RecipeResponse>>(responseList);
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

        #region Get by ID
        public async Task<ResponseObject<RecipeResponse>> GetRecipeById(string id)
        {
            var result = new ResponseObject<RecipeResponse>();
            var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(id);
            if (recipe != null)
            {
                result.StatusCode = 200;
                result.Message = "Recipe with Id " + id + ":";
                RecipeResponse response = _mapper.Map<RecipeResponse>(recipe);
                result.Data = response;
                return result;
            }
            else
            {
                result.StatusCode = 404;
                result.Message = "Not found. Data not found or wrong id";
                return result;
            }
            throw new NotImplementedException();
        }
        #endregion

        #region Get by name
        public async Task<ResponseObject<RecipeResponse>> GetRecipeByName(string name)
        {
            var result = new ResponseObject<RecipeResponse>();
            var currentList = await _unitOfWork.RecipeRepository.GetAllAsync();
            if (currentList != null && currentList.Count() > 0)
            {
                //var foundList = ingredientList.Where(x => x.Name.Contains(name)).ToList();
                var foundList = currentList.Where(x => x.Name.StartsWith(name)).ToList();
                if (foundList == null)
                {
                    result.StatusCode = 404;
                    result.Message = "Not found. No such recipe in collection contain keyword: " + name;
                    return result;

                }
                else
                {
                    result.StatusCode = 200;
                    result.Message = "Ingredient list found by name";
                    result.List = _mapper.Map<List<RecipeResponse>>(foundList);
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

        #region Create
        #endregion

        #region Update
        #endregion

        #region Change status
        public async Task<ResponseObject<RecipeResponse>> ChangeStatus(ChangeRecipeStatusRequest recipe)
        {
            var result = new ResponseObject<RecipeResponse>();
            var validateResult = _recipeChangeStatusValidator.Validate(recipe);
            if (!validateResult.IsValid)
            {
                var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
                result.StatusCode = 400;
                result.Message = string.Join(" - ", error);
                return result;
            }
            var found = await _unitOfWork.RecipeRepository.GetByIdAsync(recipe.Id.ToString());
            if (found == null)
            {
                result.StatusCode = 400;
                result.Message = "Not found recipe id " + recipe.Id;
                return result;
            }
            var changeResult = await _unitOfWork.RecipeRepository.ChangeStatusAsync(recipe.Id, recipe.ProcessStatus);
            if (changeResult){
                await _unitOfWork.CompleteAsync();
                result.StatusCode = 200;
                result.Message = "Change status success";
                return result;
            }
            else
            {
                result.StatusCode = 400;
                result.Message = "Change Ingredient " + recipe.Id + " status Unsuccessfully";
                return result;
            }
        }
        #endregion

        #region Delete
        public async Task<ResponseObject<RecipeResponse>> DeleteRecipeById(IdRecipeRequest recipe)
        {
            var result = new ResponseObject<RecipeResponse>();
            var validateResult = _idValidator.Validate(recipe);
            if (validateResult != null)
            {
                var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
                result.StatusCode = 400;
                result.Message = string.Join(" - ", error);
                return result;
            }
            var found = await _unitOfWork.RecipeRepository.GetByIdAsync(recipe.Id.ToString());
            if (found == null)
            {
                result.StatusCode = 500;
                result.Message = "not found";
                return result;
            }
            else
            {
                var deleteResult = await _unitOfWork.RecipeRepository.DeleteAsync(recipe.Id.ToString());
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
                    result.Message = "Error at delete RECIPE";
                    return result;
                }
            }
        }
        #endregion
    }
}
