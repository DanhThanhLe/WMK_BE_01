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

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class RecipeCategoryService : IRecipeCategoryService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public RecipeCategoryService(IUnitOfWork unitOfWork , IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		List<string> categoryTypeList = new List<string> { "Nation" , "Classify" , "Cooking Method" , "Meal in day" };
		public async Task<ResponseObject<List<RecipeCategory>?>> Create(Guid recipeId , List<Guid> categoryIdList)
		{
			var result = new ResponseObject<List<RecipeCategory>?>();
			if ( categoryIdList.Count < 4 || categoryIdList.Count > 4 )//ko du 4 category thi ko cho tao
			{
				result.StatusCode = 400;
				result.Message = "Not enough categories for recipe (standard is 4)";
				return result;
			}
			//kiem tra recipe da ton tai chua
			var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
			if ( recipeExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Recipe not exist!";
				return result;
			}

			//du roi thi gio kiem tra trung lap
			var checkCategoriesResult = CheckTypeCategories(categoryIdList);
			if ( !checkCategoriesResult )
			{
				result.StatusCode = 402;
				result.Message = "Have same type!";
				return result;
			}
			//toi luc nay thi coi nhu khong co trung lap trong db cung nhu trong request -> bat dau tao du lieu
			var currentList = await _unitOfWork.RecipeCategoryRepository.GetAllAsync();
			var foundByRecipe = currentList.Where(x => x.RecipeId == recipeId);
			if ( foundByRecipe.Count() >= categoryTypeList.Count() )//gio thi kiem tra coi recipe do đa co category chua
			{
				result.StatusCode = 400;
				result.Message = "This recipe already have enough categories";
				return result;
			}

			List<RecipeCategory>? returnList = new List<RecipeCategory>();
			foreach ( var categoryId in categoryIdList )//toi day coi nhu data dau vao da nhap ok, co the tien hanh add recipeCategory
			{
				var categoryExist = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId.ToString());
				if ( categoryExist == null )
				{
					result.StatusCode = 404;
					result.Message = "Category (" + categoryId + ") not exist!";
					return result;
				}
				RecipeCategory newRecipeCategory = new RecipeCategory();
				newRecipeCategory.RecipeId = recipeId;
				newRecipeCategory.CategoryId = categoryId;
				newRecipeCategory.Recipe = recipeExist;
				newRecipeCategory.Category = categoryExist;
				var createResult = await _unitOfWork.RecipeCategoryRepository.CreateAsync(newRecipeCategory);
				if ( !createResult )//cho nay tra ve luon thong tin cu the type nao bi tao loi
				{
					result.StatusCode = 500;
					result.Message = "Create new recipe category unsuccessfully!";
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

		private bool CheckTypeCategories(List<Guid> categoryIdList)//cai nay dung de check xem cai list category nhap vao co bi trung type hay khong
		{
			//check type co trung nhau khong
			foreach ( var type in categoryTypeList )//cho chay vong lap doi voi lai id category, neu nhu type cua category nao bi trung thi bao loi
			{
				int count = 0;
				foreach ( var categoryId in categoryIdList )
				{
					if ( _unitOfWork.CategoryRepository.GetByIdAsync(categoryId.ToString()).Equals(type) )//cai nay dem coi co type nao bi trung khong
					{
						count++;
					}
					if ( count > 1 )
					{
						//neu co type trung lap thi tra ve false
						return false;
					}
				}
			}
			return true;
		}

		public async Task<ResponseObject<RecipeCategoryResponse>> Update(RecipeCategoryRequest recipeCategory)
		{
			//tim id cua recipe
			var result = new ResponseObject<RecipeCategoryResponse>();
			var recipeCategoryList = await _unitOfWork.RecipeCategoryRepository.GetAllAsync();
			var recipe = _unitOfWork.RecipeRepository.GetById(recipeCategory.RecipeId);
			if ( recipe == null )
			{
				result.StatusCode = 400;
				result.Message = "Not found recipe";
				return result;
			}
			else
			{

			}
			result.StatusCode = 200;
			result.Message = "test";
			return result;
		}
		//public async Task<ResponseObject<RecipeCategoryResponse>> GetByRecipeId(Guid recipeId)
		//{
		//    var result  = new ResponseObject<RecipeCategoryResponse>();
		//    List<RecipeCategory> currentList = await _unitOfWork.RecipeCategoryRepository.GetAllAsync();
		//    List<RecipeCategory> listFound = (List<RecipeCategory>)currentList.ToList().Where(x => x.RecipeId == recipeId);
		//    if(listFound.Count() > 0)
		//    {
		//        result.StatusCode = 400;
		//        result.Message = "Not found. At GetByRecipeId. RecipeCategory service";
		//        result.List = _mapper.Map<RecipeCategory>(listFound);
		//        return result;
		//    }
		//}
	}
        
}
