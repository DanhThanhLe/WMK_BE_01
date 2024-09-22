using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
		
		#region Get-all
		public async Task<ResponseObject<List<RecipeCategoryResponse>>> GetAll(string name = "")
		{
			var result = new ResponseObject<List<RecipeCategoryResponse>>();
			var list = await _unitOfWork.RecipeCategoryRepository.GetAllAsync();
			list = list.Where(x => x.Recipe.Name.ToLower().RemoveDiacritics().Contains(name.ToLower().RemoveDiacritics())).ToList();
			if ( list == null || list.Count == 0 )
			{
				result.StatusCode = 400;
				result.Message = "Danh sách trống";
				return result;
			}
			result.StatusCode = 200;
			result.Message = "Tìm thấy";
			result.Data = _mapper.Map<List<RecipeCategoryResponse>>(list);
			return result;
		}
		#endregion Get-all

		#region Get-by-recipe-id
		public async Task<ResponseObject<List<RecipeCategoryResponse>>> GetListByRecipeId(Guid recipeId)
		{
			var result = new ResponseObject<List<RecipeCategoryResponse>>();
			var checkRecipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
			if ( checkRecipe == null )
			{
				result.StatusCode = 400;
				result.Message = "Công thức không tồn tại";
				return result;
			}
			var currentList = await _unitOfWork.RecipeCategoryRepository.GetAllAsync();
			var foundList = new List<RecipeCategory>();
			foreach ( var item in currentList )
			{
				if ( item.RecipeId.Equals(recipeId) )
				{
					foundList.Add(item);
				}
			}
			if ( foundList.Count == 0 )
			{
				result.StatusCode = 500;
				result.Message = "Không tìm thấy";
				return result;
			}
			result.StatusCode = 200;
			result.Message = "Tìm thấy";
			result.Data = _mapper.Map<List<RecipeCategoryResponse>>(foundList);
			return result;
		}

		#endregion

		#region Get-recipe-list-by-category-id
		public List<Guid> GetRecipeIdByCategoryId(Guid categoryId)
		{
			var recipeCategoryFoundList = _unitOfWork.RecipeCategoryRepository.Get(x => x.CategoryId == categoryId);
			if ( recipeCategoryFoundList == null )
			{
				return null;
			}
			List<Guid> result = new List<Guid>();
			foreach ( var item in recipeCategoryFoundList )
			{
				result.Add(item.RecipeId);
			}
			return result;
		}
		#endregion Get-recipe-list-by-category-id

		#region Create-for-recipeCreate
		public async Task<ResponseObject<List<RecipeCategory>?>> Create(Guid recipeId , List<Guid> categoryIdList)
		{
			var result = new ResponseObject<List<RecipeCategory>?>();
			if ( categoryIdList.Count < 4 || categoryIdList.Count > 4 )//ko du 4 category thi ko cho tao
			{
				result.StatusCode = 400;
				result.Message = "Cần đủ 4 category cho 1 công thức";
				return result;
			}
			//kiem tra recipe da ton tai chua
			var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
			if ( recipeExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Công thức không tổn tại";
				return result;
			}

			//du roi thi gio kiem tra trung lap
			var checkCategoriesResult = CheckTypeCategories(categoryIdList);
			if ( !checkCategoriesResult )
			{
				result.StatusCode = 400;
				result.Message = "Trùng type Category";
				return result;
			}

			// phan nay co ve khong can

			//toi luc nay thi coi nhu khong co trung lap trong db cung nhu trong request -> bat dau tao du lieu
			var currentList = await _unitOfWork.RecipeCategoryRepository.GetAllAsync();
			var foundByRecipe = currentList.Where(x => x.RecipeId == recipeId);
			if ( foundByRecipe.Count() >= categoryTypeList.Count() )//gio thi kiem tra coi recipe do đa co category chua
			{
				//trường hợp đã có recipe category
				result.StatusCode = 400;
				result.Message = "Công thức đã quy định đủ category";
				return result;
			}

			//tạo recipe category

			List<RecipeCategory>? returnList = new List<RecipeCategory>();
			foreach ( var categoryId in categoryIdList )//toi day coi nhu data dau vao da nhap ok, co the tien hanh add recipeCategory
			{
				var categoryExist = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId.ToString());
				if ( categoryExist == null )
				{
					result.StatusCode = 404;
					result.Message = "Category Không tồn tại";
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
					result.Message = "Tạo không thành công";
					return result;
				}
				returnList.Add(newRecipeCategory);
			}
			await _unitOfWork.CompleteAsync();
			result.StatusCode = 200;
			result.Message = "Tạo thành công";
			result.Data = returnList;
			return result;
		}
		#endregion Create-for-recipe-create

		#region Update
		public async Task<ResponseObject<RecipeCategoryResponse>> Update(RecipeCategoryRequest recipeCategory)
		{
			//tim id cua recipe
			var result = new ResponseObject<RecipeCategoryResponse>();
			//var recipeCategoryList = await _unitOfWork.RecipeCategoryRepository.GetAllAsync();
			var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeCategory.RecipeId.ToString());
			if ( recipe == null )
			{
				result.StatusCode = 400;
				result.Message = "Không tìm thấy bản ghi";
				return result;
			}
			else
			{
				result.StatusCode = 200;
				result.Message = "Tìm thấy";
				result.Data = _mapper.Map<RecipeCategoryResponse>(recipeCategory);
				return result;
			}

		}
		#endregion Update

		#region Delete
		public async Task<ResponseObject<List<RecipeCategory>?>> DeleteByRcipe(Guid recipeId)
		{
			var result = new ResponseObject<List<RecipeCategory>?>();
			try
			{
				//check recipe exist
				var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
				if ( recipeExist == null )
				{
					result.StatusCode = 404;
					result.Message = "Công thức không tồn tại";
					return result;
				}
				//take recipeCategory asign to recipe
				var recipeCategories = _unitOfWork.RecipeCategoryRepository.GetAll()
					.Where(rc => rc.RecipeId == recipeId);
				if ( recipeCategories == null || !recipeCategories.Any() )
				{
					result.StatusCode = 200;
					result.Message = "Không tìm thấy category tương ứng";
					return result;
				}
				// Xóa các RecipeCategory liên quan
				_unitOfWork.RecipeCategoryRepository.DeleteRange(recipeCategories);
				await _unitOfWork.CompleteAsync();

				result.StatusCode = 200;
				result.Message = "Xóa thành công";
				result.Data = recipeCategories.ToList();
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
			}

			return result;
		}
		#endregion
		
		#region Check-type-categories <private>
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
		#endregion Check-type-categories
	}
}
