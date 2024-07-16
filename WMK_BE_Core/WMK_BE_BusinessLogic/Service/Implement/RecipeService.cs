using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
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
        private readonly IRecipeIngredientService _recipeAmountService;
        private readonly RecipeValidator _validator;
        private readonly IMapper _mapper;
        private readonly RecipeChangeStatusValidator _recipeChangeStatusValidator;
        private readonly IdRecipeValidator _idValidator;
        private readonly IRecipeCategoryService _recipeCategoryService;
        private readonly IRecipeStepService _recipeStepService;
        private readonly IRecipeNutrientService _recipeNutrientService;
        private readonly IRecipeIngredientService _recipeIngredientService;
        public RecipeService(IUnitOfWork unitOfWork, IMapper mapper, IRecipeIngredientService recipeAmountService, IRecipeCategoryService recipeCategoryService, IRecipeNutrientService recipeNutrientService, IRecipeIngredientService recipeIngredientService, IRecipeStepService recipeStepService)
        {
            _unitOfWork = unitOfWork;
            _recipeAmountService = recipeAmountService;
            _mapper = mapper;
            _validator = new RecipeValidator();
            _recipeChangeStatusValidator = new RecipeChangeStatusValidator();
            _idValidator = new IdRecipeValidator();
            _recipeCategoryService = recipeCategoryService;
            _recipeNutrientService = recipeNutrientService;
            _recipeIngredientService = recipeIngredientService;
            _recipeStepService = recipeStepService;
        }


        #region Get all
        public async Task<ResponseObject<List<RecipeResponse>>> GetRecipes()//ham nay ko dung cho user
        {
            var result = new ResponseObject<List<RecipeResponse>>();
            var currentList = await _unitOfWork.RecipeRepository.GetAllAsync();
            var responseList = currentList.ToList().Where(x => x.ProcessStatus == ProcessStatus.Approved);
            if (currentList != null && currentList.Count() > 0)
            {
                result.StatusCode = 200;
                result.Message = "OK. Recipe list "+responseList.Count() ;
                result.Data = _mapper.Map<List<RecipeResponse>>(responseList);
                return result;
            }
            else
            {
                result.StatusCode = 500;
                result.Message = "Not found. Empty list or Data not found. Say from GetRecipes - RecipeService";
                return result;
            }
        }
        #endregion

        #region Get by ID
        public async Task<ResponseObject<RecipeResponse>> GetRecipeById(string id)
        {
            var result = new ResponseObject<RecipeResponse>();
            var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(id);
            if (recipe != null && recipe.Id.ToString() != null)
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
        public async Task<ResponseObject<RecipeResponse>> GetRecipeByName(string name)//ham nay hien tai cho coi tat ca recipe bat ke status
        {
            var result = new ResponseObject<RecipeResponse>();
            var currentList = await _unitOfWork.RecipeRepository.GetAllAsync();
            if (currentList != null && currentList.Count() > 0)
            {
                var foundList = currentList.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
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

        #region Search
        #endregion

        #region Create
        public async Task<ResponseObject<RecipeResponse>> CreateRecipeAsync(CreateRecipeRequest recipe)
        {
            var result = new ResponseObject<RecipeResponse>();
            var currentList = await _unitOfWork.RecipeRepository.GetAllAsync();
            try
            {
                var validateResult = _validator.Validate(recipe);
                if (!validateResult.IsValid)
                {
                    var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
                    result.StatusCode = 500;
                    result.Message = "Say from CreateRecipeAsync - RecipeService./n" + string.Join(" - /n", error);
                    return result;
                }
                //mapper
                Recipe newRecipe = _mapper.Map<Recipe>(recipe);
                newRecipe.Popularity = 0;
                newRecipe.CreatedAt = DateTime.Now;
                newRecipe.UpdatedAt = DateTime.Now;
                newRecipe.ProcessStatus = ProcessStatus.Processing;
                var checkDuplicateName = currentList.FirstOrDefault(x => x.Name.ToLower().Equals(recipe.Name.ToLower()));
                if (checkDuplicateName != null)
                {
                    result.StatusCode = 400;
                    result.Message = "Duplicate name with ID " + checkDuplicateName.Id.ToString();
                    return result;
                }
                //tao gia co ban cho recipe dua vao gia don vi cua nguyen lieu
                foreach (var item in recipe.RecipeIngredientsList)
                {
                    var ingredientFound = await _unitOfWork.IngredientRepository.GetByIdAsync(item.IngredientId.ToString());
                    if (ingredientFound != null)
                    {
                        newRecipe.Price += ingredientFound.Price * item.amount;
                    }
                    else
                    {
                        result.StatusCode = 400;
                        result.Message = "Ingredient ID " + item.IngredientId + " not found.";
                        return result;
                    }
                }

                var createResult = await _unitOfWork.RecipeRepository.CreateAsync(newRecipe);
                if (!createResult)
                {
                    result.StatusCode = 500;
                    result.Message = "Create Recipe unsuccessfully! Say from CreateRecipeAsync - RecipeService.";
                    return result;
                }
                await _unitOfWork.CompleteAsync();

                //bat dau tao cac thanh phan lien quan

                //create RecipeCategory
                var checkCreateRecipeCategory = await _recipeCategoryService.Create(newRecipe.Id, recipe.CategoryIds);

                //create RecipeIngredient
                var checkCreateRecipeIngredient = await _recipeIngredientService.CreateRecipeIngredientAsync(newRecipe.Id, recipe.RecipeIngredientsList);

                //create RecipeStep
                var checkCreateRecipeStep = await _recipeStepService.CreateRecipeSteps(newRecipe.Id, recipe.Steps);

                //create RecipeNutrient
                var checkCreateRecipeNutrient = await _recipeNutrientService.Create(newRecipe.Id, recipe.RecipeIngredientsList);

                if (//1 trong 3 cai ko tao dc thi xoa thong tin hien hanh cua recipe moi dang tao
                    checkCreateRecipeCategory.StatusCode != 200 || checkCreateRecipeCategory.Data == null
                    || checkCreateRecipeIngredient.StatusCode != 200 || checkCreateRecipeIngredient.Data == null
                    || checkCreateRecipeStep.StatusCode != 200 || checkCreateRecipeStep.Data == null
                    || checkCreateRecipeNutrient.StatusCode != 200 || checkCreateRecipeNutrient.Data == null
                    )
                {
                    resetRecipe(newRecipe.Id);
                    result.StatusCode = 500;
                    result.Message = checkCreateRecipeCategory.Message
                        + " | " + checkCreateRecipeIngredient.Message
                        + " | " + checkCreateRecipeIngredient.Message
                        + " | " + checkCreateRecipeNutrient.Message;
                    return result;
                }
                else//ko co loi va hoan thanh tao moi
                {
                    await _unitOfWork.CompleteAsync();
                    result.StatusCode = 200;
                    result.Message = "Create Recipe successfully.";
                    return result;
                }
                #region old
                //newRecipe.RecipeCategories = checkCreateRecipeCategory.Data.ToList();
                //create RecipeCategory

                //if ()
                //{
                //    resetRecipe(newRecipe.Id);
                //    result.StatusCode = checkCreateRecipeIngredient.StatusCode;
                //    result.Message = ;
                //    return result;
                //}
                //newRecipe.RecipeIngredients = checkCreateRecipeIngredient.Data.ToList();
                ////create RecipeIngredient



                //if ()
                //{
                //    resetRecipe(newRecipe.Id);
                //    result.StatusCode = checkCreateRecipeIngredient.StatusCode;
                //    result.Message = ;
                //    return result;
                //}
                //newRecipe.RecipeSteps = checkCreateRecipeStep.Data.ToList();
                ////create RecipeStep


                //if ()
                //{
                //    resetRecipe(newRecipe.Id);
                //    result.StatusCode = checkCreateRecipeIngredient.StatusCode;
                //    result.Message = checkCreateRecipeIngredient.Message;
                //    return result;
                //}
                //else
                //{
                //    await _unitOfWork.CompleteAsync();
                //}
                #endregion
            }
            catch (Exception ex)
            {
                result.StatusCode = 500;
                result.Message = ex.Message;
                return result;
            }
        }

        //reset Recipe
        private void resetRecipe(Guid recipeId)
        {
            _unitOfWork.RecipeRepository.DeleteAsync(recipeId.ToString());
        }

        #endregion

        #region Update (26/05/2024)
        public async Task<ResponseObject<RecipeResponse>> Update(RecipeRequest updateRecipe)
        {
            /*
			tim recipe
			lay du lieu goc
			thay doi cho cac thong so
			giu nguyen cho cac thong so bo trong
			 */
            var result = new ResponseObject<RecipeResponse>();
            var foundRecipe = await _unitOfWork.RecipeRepository.GetByIdAsync(updateRecipe.Id.ToString());
            if (foundRecipe != null)
            {
                result.StatusCode = 400;
                result.Message = "Not found recipe";
                return result;
            }
            var currentList = await (_unitOfWork.RecipeRepository.GetAllAsync());
            var duplicateName = currentList.FirstOrDefault(x => x.Name == updateRecipe.Name);
            if (duplicateName != null
                && duplicateName.Id != updateRecipe.Id
                && duplicateName.ProcessStatus == ProcessStatus.Processing)
            {
                result.StatusCode = 400;
                result.Message = "Recipe with name: " + updateRecipe.Name + " is already existed";
                return result;
            }
            if (updateRecipe.Name != null)
            {
                foundRecipe.Name = updateRecipe.Name;
            }
            if (updateRecipe.ServingSize != null)
            {
                foundRecipe.ServingSize = updateRecipe.ServingSize;
            }
            if (updateRecipe.Difficulty != null)
            {
                foundRecipe.Difficulty = updateRecipe.Difficulty;
            }
            if (updateRecipe.Description != null)
            {
                foundRecipe.Description = updateRecipe.Description;
            }
            if (updateRecipe.ImageLink != null)
            {
                foundRecipe.Img = updateRecipe.ImageLink;
            }
            if (updateRecipe.Price != null)
            {
                foundRecipe.Price = updateRecipe.Price;
            }
            if (updateRecipe.ApprovedBy != null)
            {
                foundRecipe.ApprovedBy = updateRecipe.ApprovedBy;
            }
            if (updateRecipe.ApprovedAt != null)
            {
                foundRecipe.ApprovedAt = updateRecipe.ApprovedAt;
            }
            if (updateRecipe.UpdatedBy != null)
            {
                foundRecipe.UpdatedBy = updateRecipe.UpdatedBy;
            }
            foundRecipe.UpdatedAt = DateTime.Now;
            if (updateRecipe.Popularity != null)
            {
                foundRecipe.Popularity = updateRecipe.Popularity;
            }
            if (updateRecipe.ProcessStatus != null)
            {
                foundRecipe.ProcessStatus = updateRecipe.ProcessStatus;
            }
            var updateResult = await _unitOfWork.RecipeRepository.UpdateAsync(foundRecipe);
            if (!updateResult)
            {
                result.StatusCode = 500;
                result.Message = "Error when updating recipe in recipe service using updateAsync";
                return result;
            }
            result.StatusCode = 500;
            result.Message = "Update recipe id " + updateRecipe.Id + " done";
            return result;
        }
        #endregion

        #region Change status -- just manager use
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
                result.StatusCode = 404;
                result.Message = "Not found recipe id " + recipe.Id + "!";
                return result;
            }
            var changeResult = await _unitOfWork.RecipeRepository.ChangeStatusAsync(recipe.Id, recipe.ProcessStatus);
            if (changeResult)
            {
                await _unitOfWork.CompleteAsync();
                result.StatusCode = 200;
                result.Message = "Change status success";
                return result;
            }
            else
            {
                result.StatusCode = 500;
                result.Message = "Change Recipe " + recipe.Id + " status Unsuccessfully!";
                return result;
            }
        }
        #endregion

        #region Delete
        public async Task<ResponseObject<RecipeResponse>> DeleteRecipeById(Guid request)
        {
            var result = new ResponseObject<RecipeResponse>();
            var found = await _unitOfWork.RecipeRepository.GetByIdAsync(request.ToString());
            if (found == null)
            {
                result.StatusCode = 404;
                result.Message = "Not found recipe!";
                return result;
            }


            //check recipe exist in weekly plan - if have, just change status -> cancel
            var deleteResult = await _unitOfWork.RecipeRepository.DeleteAsync(request.ToString());
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
        #endregion

        #region Search recipe list with recipe category id
        public async Task<ResponseObject<RecipeResponse>> GetListByCategoryId(Guid categoryId)
        {
            /*
			 1-lay list recipe category lien quan
				1.1 - xac dinh category co ton tai (category repo - get by id)
				1.2 - tim list recipe category lien quan
					+ - tao list trong
					+ - cho chay vong for gap dung thi add recipe id
			 2-tim list recipe lien quan
				2.1 - tao list trong
				2.2 - cho chay vong for gap dung thi add
			 3- tra ve 200 va list recipe
			 */
            var result = new ResponseObject<RecipeResponse>();
            var checkCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId.ToString());
            if (checkCategory == null)
            {
                result.StatusCode = 400;
                result.Message = "Wrong category. Not found Category";
                return result;
            }
            var recipeIdListfound = _recipeCategoryService.GetRecipeIdByCategoryId(categoryId);
            if (recipeIdListfound == null)//cai nay la coi nhu tim ko co mon an thich hop, chu ko phai loi
            {
                result.StatusCode = 200;
                result.Message = "Not found suitable recipe";
                result.List = new List<RecipeResponse>();
                return result;
            }
            List<Recipe> listRecipe = new List<Recipe>();
            foreach (var item in recipeIdListfound)
            {
                var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(item.ToString());
                listRecipe.Add(recipe);
            }
            result.StatusCode = 200;
            result.Message = "Recipe list base on categoryID: ";
            result.List = _mapper.Map<List<RecipeResponse>>(listRecipe);
            return result;
        }
        #endregion
    }
}
