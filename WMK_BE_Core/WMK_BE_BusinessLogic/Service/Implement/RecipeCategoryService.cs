using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeCategoryModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class RecipeCategoryService : IRecipeCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RecipeCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        List<string> categoryTypeList = new List<string> { "Nation", "Classify", "Cooking Method", "Meal in day" };

        #region Create-for-recipe-create
        public async Task<ResponseObject<List<RecipeCategory>?>> Create(Guid recipeId, List<Guid> categoryIdList)
        {
            var result = new ResponseObject<List<RecipeCategory>?>();
            if (categoryIdList.Count < 4 || categoryIdList.Count > 4)//ko du 4 category thi ko cho tao
            {
                result.StatusCode = 400;
                result.Message = "Not enough categories for recipe (standard is 4)";
                return result;
            }
            //kiem tra recipe da ton tai chua
            var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
            if (recipeExist == null)
            {
                result.StatusCode = 404;
                result.Message = "Recipe not exist! Say from Create - RecipeCategoryService";
                return result;
            }

            //du roi thi gio kiem tra trung lap
            var checkCategoriesResult = CheckTypeCategories(categoryIdList);
            if (!checkCategoriesResult)
            {
                result.StatusCode = 400;
                result.Message = "Have same type!";
                return result;
            }

            // phan nay co ve khong can

            //toi luc nay thi coi nhu khong co trung lap trong db cung nhu trong request -> bat dau tao du lieu
            var currentList = await _unitOfWork.RecipeCategoryRepository.GetAllAsync();
            var foundByRecipe = currentList.Where(x => x.RecipeId == recipeId);
            if (foundByRecipe.Count() >= categoryTypeList.Count())//gio thi kiem tra coi recipe do đa co category chua
            {
                result.StatusCode = 400;
                result.Message = "This recipe already have enough categories. Say from Create - RecipeCategoryService";
                return result;
            }

            //phan nay co ve khong can

            List<RecipeCategory>? returnList = new List<RecipeCategory>();
            foreach (var categoryId in categoryIdList)//toi day coi nhu data dau vao da nhap ok, co the tien hanh add recipeCategory
            {
                var categoryExist = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId.ToString());
                if (categoryExist == null)
                {
                    result.StatusCode = 404;
                    result.Message = "Category (" + categoryId + ") not exist! Say from Create - RecipeCategoryService";
                    return result;
                }
                RecipeCategory newRecipeCategory = new RecipeCategory();
                newRecipeCategory.RecipeId = recipeId;
                newRecipeCategory.CategoryId = categoryId;
                newRecipeCategory.Recipe = recipeExist;
                newRecipeCategory.Category = categoryExist;
                var createResult = await _unitOfWork.RecipeCategoryRepository.CreateAsync(newRecipeCategory);
                if (!createResult)//cho nay tra ve luon thong tin cu the type nao bi tao loi
                {
                    result.StatusCode = 500;
                    result.Message = "Create new recipe category unsuccessfully! Say from Create - RecipeCategoryService";
                    result.Data = null;
                    return result;
                }
                await _unitOfWork.CompleteAsync();
                returnList.Add(newRecipeCategory);
            }
            result.StatusCode = 200;
            result.Message = "Create recipeCategory successfully";
            result.Data = returnList;
            return result;
        }
        #endregion Create-for-recipe-create

        #region Check-type-categories <private>
        private bool CheckTypeCategories(List<Guid> categoryIdList)//cai nay dung de check xem cai list category nhap vao co bi trung type hay khong
        {
            //check type co trung nhau khong
            foreach (var type in categoryTypeList)//cho chay vong lap doi voi lai id category, neu nhu type cua category nao bi trung thi bao loi
            {
                int count = 0;
                foreach (var categoryId in categoryIdList)
                {
                    if (_unitOfWork.CategoryRepository.GetByIdAsync(categoryId.ToString()).Equals(type))//cai nay dem coi co type nao bi trung khong
                    {
                        count++;
                    }
                    if (count > 1)
                    {
                        //neu co type trung lap thi tra ve false
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion Check-type-categories

        #region Update
        public async Task<ResponseObject<RecipeCategoryResponse>> Update(RecipeCategoryRequest recipeCategory)
        {
            //tim id cua recipe
            var result = new ResponseObject<RecipeCategoryResponse>();
            //var recipeCategoryList = await _unitOfWork.RecipeCategoryRepository.GetAllAsync();
            var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeCategory.RecipeId.ToString());
            if (recipe == null)
            {
                result.StatusCode = 400;
                result.Message = "Not found recipe";
                return result;
            }
            else
            {
                result.StatusCode = 200;
                result.Message = "Ok recipeCategory";
                result.Data = _mapper.Map<RecipeCategoryResponse>(recipeCategory);
                return result;
            }

        }
        #endregion Update
        //public async Task<ResponseObject<RecipeCategoryResponse>> GetByRecipeId(Guid recipeId)
        //{
        //    var result = new ResponseObject<RecipeCategoryResponse>();
        //    List<RecipeCategory> currentList = await _unitOfWork.RecipeCategoryRepository.GetAllAsync();
        //    List<RecipeCategory> listFound = (List<RecipeCategory>)currentList.ToList().Where(x => x.RecipeId == recipeId);
        //    if (listFound.Count() > 0)
        //    {
        //        result.StatusCode = 400;
        //        result.Message = "Not found. At GetByRecipeId. RecipeCategory service";
        //        result.List = _mapper.Map<RecipeCategory>(listFound);
        //        return result;
        //    }
        //}
        #region Get-recipe-list-by-category-id
        public List<Guid> GetRecipeIdByCategoryId(Guid categoryId)
        {
            var recipeCategoryFoundList = _unitOfWork.RecipeCategoryRepository.Get(x => x.CategoryId == categoryId);
            if (recipeCategoryFoundList == null)
            {
                return null;
            }
            List<Guid> result = new List<Guid>();
            foreach (var item in recipeCategoryFoundList)
            {
                result.Add(item.RecipeId);
            }
            return result;
        }
        #endregion Get-recipe-list-by-category-id

        #region Get-all
        public async Task<ResponseObject<RecipeCategoryResponse>> GetAll()
        {
            var result = new ResponseObject<RecipeCategoryResponse>();
            var list = await _unitOfWork.RecipeCategoryRepository.GetAllAsync();
            if (list == null || list.Count == 0)
            {
                result.StatusCode = 400;
                result.Message = "Not found. Empty list";
                return result;
            }
            result.StatusCode = 200;
            result.Message = "OK. list recipe cateory";
            result.List = _mapper.Map<List<RecipeCategoryResponse>>(list);
            return result;
        }
        #endregion Get-all

        #region Get-by-recipe-id
        public async Task<ResponseObject<RecipeCategoryResponse>> GetListByRecipeId(Guid recipeId)
        {
            var result = new ResponseObject<RecipeCategoryResponse>();
            var checkRecipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
            if (checkRecipe == null)
            {
                result.StatusCode = 400;
                result.Message = "Recipe not existed";
                return result;
            }
            var currentList = await _unitOfWork.RecipeCategoryRepository.GetAllAsync();
            var foundList = new List<RecipeCategory>();
            foreach (var item in currentList)
            {
                if (item.RecipeId.Equals(recipeId))
                {
                    foundList.Add(item);
                }
            }
            if (foundList.Count == 0)
            {
                result.StatusCode = 500;
                result.Message = "Not found List for Id: " + recipeId;
                return result;
            }
            result.StatusCode = 200;
            result.Message = "Ok. Recipe category list:";
            result.List = _mapper.Map<List<RecipeCategoryResponse>>(foundList);
            return result;
        }
        #endregion
    }

}
