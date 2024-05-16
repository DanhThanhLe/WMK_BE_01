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

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class RecipeCategoryService : IRecipeCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RecipeCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        List<string> categoryTypeLists = new List<string> { "Nation", "Classify", "Cooking Method" };
        public async Task<ResponseObject<RecipeCategoryResponse>> Create(CreateRecipeCategoryRequest recipeCategory)
        {
            /*
             kiem tra recipe ton tai
             kiem tra category ton tai
             quy dinh recipe co 3 category -> 3 type
            kiem tra cap recipe-category ton tai chua
            kiem tra type cua category do gan vs recipe dang lam chua -> co roi thi bao loi
            ()con tiep
             */
            var result = new ResponseObject<RecipeCategoryResponse>();
            
            var recipeFound = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeCategory.RecipeId.ToString());
            var categoryFound = await _unitOfWork.CategoryRepository.GetByIdAsync(recipeCategory.CategoryId.ToString());
            var currentList = await _unitOfWork.RecipeCategoryRepository.GetAllAsync();//lay list recipe category

            var recipeCategoryList = currentList.ToList().Where(x => x.RecipeId == recipeCategory.RecipeId);//lay list hien co cua recipecategory ung voi recipe id
            
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(recipeCategory.CategoryId.ToString());
            foreach (var item in recipeCategoryList)
            {
                var type = _unitofwork.categoryrepository.get(x => x.type == cate)
                if (item.categoryid)
            }

            if (recipeFound == null || categoryFound == null)
            {
                result.StatusCode = 400;
                result.Message = "Error at create recipe ingredient detail info at recipeCategory";
                return result;
            }
            //Category NationCategory = null;
            //Category ClassifyCategory = null;
            //Category CookingMethodCategory = null;
            List<Category> categories = new List<Category>();
            foreach ( var item in recipeCategoryList )
            {
                if(item.)
            }
            //else
            //{
            //    var currentListForRecipe = _unitOfWork.RecipeCategoryRepository.Get(x => x.RecipeId == recipeCategory.RecipeId);
            //    List<Category> categories = new List<Category>();
            //    foreach (var item in currentListForRecipe)
            //    {
            //        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(item.CategoryId.ToString());
            //        categories.Add(category);
            //    }//da lay duoc danh sach category cua recipe do

            //    //bat dau kiem tra 3 category moi recipe
            //}


            throw new NotImplementedException();
        }

        public Task<ResponseObject<RecipeCategoryResponse>> Update(RecipeCategoryRequest recipeCategory)
        {
            throw new NotImplementedException();
        }
    }
}
