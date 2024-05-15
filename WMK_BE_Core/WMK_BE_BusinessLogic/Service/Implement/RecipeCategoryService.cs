using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeCategoryModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
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


            throw new NotImplementedException();
        }

        public Task<ResponseObject<RecipeCategoryResponse>> Update(RecipeCategoryRequest recipeCategory)
        {
            throw new NotImplementedException();
        }
    }
}
