using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.CategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.CategoryModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using System.Net;
using Microsoft.IdentityModel.Tokens;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class CategoryService : ICategoryService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public CategoryService(IUnitOfWork unitOfWork , IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		List<string> categoryTypeList = new List<string> { "Nation" , "Classify" , "Cooking Method" , "Meal in day" };

		#region Get
		public async Task<ResponseObject<List<CategoryResponseModel>>> GetAllAsync(GetAllCategoriesRequest? model)
		{
			var result = new ResponseObject<List<CategoryResponseModel>>();
			var categories = new List<Category>();
			var categoriesResponse = new List<CategoryResponseModel>();
			if ( model != null && (!model.Name.IsNullOrEmpty() || !model.Type.IsNullOrEmpty()) )
			{
				if ( model != null && !model.Name.IsNullOrEmpty() )
				{
					var categoriesByName = await GetcategoriesByNameAsync(model.Name);
					if ( categoriesByName != null && categoriesByName.Data != null )
					{
						categoriesResponse.AddRange(categoriesByName.Data);
					}
				}
				if ( model != null && !model.Type.IsNullOrEmpty() )
				{
					var categoriesByType = await GetCategoriesByTypeAsync(model.Type);
					if ( categoriesByType != null && categoriesByType.Data != null )
					{
						categoriesResponse.AddRange(categoriesByType.Data);
					}
				}
				// Loại bỏ các phần tử trùng lặp dựa trên Id
				categoriesResponse = categoriesResponse
					.GroupBy(c => c.Id)
					.Select(g => g.First())
					.ToList();
			}
			else
			{
				categories = await _unitOfWork.CategoryRepository.GetAllAsync();
				categoriesResponse = _mapper.Map<List<CategoryResponseModel>>(categories);
			}
			if ( categoriesResponse != null && categoriesResponse.Any() )
			{
				result.StatusCode = 200;
				result.Message = "Categories get success (" + categoriesResponse.Count + ")";
				result.Data = categoriesResponse;
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Don't have Categories!";
				result.Data = [];
				return result;
			}
		}
		public async Task<ResponseObject<List<CategoryResponseModel>>> GetCategoriesByTypeAsync(string type)
		{
			var result = new ResponseObject<List<CategoryResponseModel>>();
			var checkMatchTypeResult = CheckMatchType(type);
			if ( !checkMatchTypeResult )
			{
				result.StatusCode = 400;
				result.Message = "Not match pre-set category types!";
				return result;
			}
			var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
			categories = categories.Where(c => c.Type.ToLower().RemoveDiacritics().Contains(type.ToLower().RemoveDiacritics())).ToList();
			if ( categories != null && categories.Any() )
			{
				result.StatusCode = 200;
				result.Message = "List category with success";
				result.Data = _mapper.Map<List<CategoryResponseModel>>(categories);
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Not have list category with type!";
			return result;
		}

		public async Task<ResponseObject<List<CategoryResponseModel>>> GetcategoriesByNameAsync(string name)
		{
			var result = new ResponseObject<List<CategoryResponseModel>>();
			var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
			categories = categories.Where(x => x.Name.RemoveDiacritics().ToLower().Contains(name.RemoveDiacritics().ToLower())).ToList();
			if ( categories.Any() )
			{
				result.StatusCode = 200;
				result.Message = "List category with name contains " + name;
				result.Data = _mapper.Map<List<CategoryResponseModel>>(categories);
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Not found with category name: " + name;
			return result;

		}
		public async Task<ResponseObject<CategoryResponseModel?>> GetByIdAsync(Guid id)
		{
			var result = new ResponseObject<CategoryResponseModel?>();

			var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id.ToString());
			if ( category != null )
			{
				result.StatusCode = 200;
				result.Message = "Category: ";
				result.Data = _mapper.Map<CategoryResponseModel>(category);
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Category not found!";
				return result;
			}

		}


		#endregion
		#region Create
		public async Task<ResponseObject<CategoryResponseModel>> CreateCategoryAsync(CreateCategoryRequest model)
		{
			var result = new ResponseObject<CategoryResponseModel>();
			var newCategory = _mapper.Map<Category>(model);
			//check co type trong quy dinh
			var checkMatchTypeResult = CheckMatchType(newCategory.Type);
			if ( !checkMatchTypeResult )
			{
				result.StatusCode = 400;
				result.Message = "Not match pre-set category types!";
				return result;
			}
			//check trung ten
			var currentList = await _unitOfWork.CategoryRepository.GetAllAsync();
			var checkDuplicateNameResult = currentList.FirstOrDefault(x => x.Name.Trim() == newCategory.Name.Trim() && x.Type.Trim() == newCategory.Type.Trim());
			if ( checkDuplicateNameResult != null )
			{
				result.StatusCode = 400;
				result.Message = "Duplicate name with name: " + newCategory.Name + "!";
				return result;
			}
			newCategory.Status = BaseStatus.Available;
			var createResult = await _unitOfWork.CategoryRepository.CreateAsync(newCategory);
			if ( createResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Create new Category (" + model.Name + ") successfully.";
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Create category unsuccessfully!";
				return result;
			}
		}
		#endregion
		#region Update
		public async Task<ResponseObject<CategoryResponseModel>> UpdateCategoryAsync(Guid id , UpdateCategoryRequest model)
		{
			var result = new ResponseObject<CategoryResponseModel>();

			var categoryExist = await _unitOfWork.CategoryRepository.GetByIdAsync(id.ToString());

			if ( categoryExist == null )
			{
				result.StatusCode = 200;
				result.Message = "Category not exist!";
				return result;
			}
			//check co type trong quy dinh
			if ( model.Type != null && !model.Type.IsNullOrEmpty() )
			{
				var checkMatchTypeResult = CheckMatchType(model.Type);
				if ( !checkMatchTypeResult )
				{
					result.StatusCode = 400;
					result.Message = "Not match pre-set category types!";
					return result;
				}
			}
			if ( model.Name != null && !model.Name.IsNullOrEmpty() && model.Type != null )
			{
				//check trung ten
				var currentList = await _unitOfWork.CategoryRepository.GetAllAsync();
				var checkDuplicateNameResult = currentList.FirstOrDefault(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower() 
																			&& x.Type.Trim() == model.Type.Trim()
																			&& x.Id != id);
				if ( checkDuplicateNameResult != null )
				{
					result.StatusCode = 400;
					result.Message = "Duplicate name with name: " + model.Name + "!";
					return result;
				}
			}
			_mapper.Map(model , categoryExist);
			var updateResult = await _unitOfWork.CategoryRepository.UpdateAsync(categoryExist);
			if ( updateResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Update category name (" + categoryExist.Name + ") successfully.";
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Update category unsuccessfully!";
				return result;
			}

		}
		public async Task<ResponseObject<CategoryResponseModel>> ChangeCategoryStatusAsync(Guid id , ChangeCategoryRequest model)
		{
			var result = new ResponseObject<CategoryResponseModel>();
			//check category exist 
			var categoryExist = await _unitOfWork.CategoryRepository.GetByIdAsync(id.ToString());
			if ( categoryExist != null )
			{
				categoryExist.Status = model.Status;
				var changeResult = await _unitOfWork.CategoryRepository.UpdateAsync(categoryExist);
				if ( changeResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Change category success";
					return result;
				}
				result.StatusCode = 500;
				result.Message = "Change category unsuccess";
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Category not exist!";
			return result;

		}
		#endregion
		#region Delete
		public async Task<ResponseObject<CategoryResponseModel>> DeleteCategoryAsync(Guid id)
		{
			var result = new ResponseObject<CategoryResponseModel>();
			var categoryExist = await _unitOfWork.CategoryRepository.GetByIdAsync(id.ToString());
			if ( categoryExist == null )
			{
				result.StatusCode = 500;
				result.Message = "Category not found!";
				return result;
			}
			var recipeExistCategory = await _unitOfWork.CategoryRepository.RecipeExistCategoryAsync(id);
			if ( recipeExistCategory )
			{
				categoryExist.Status = WMK_BE_RecipesAndPlans_DataAccess.Enums.BaseStatus.UnAvailable;
				var updateResult = await _unitOfWork.CategoryRepository.UpdateAsync(categoryExist);
				if ( updateResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Change status of catgory (" + categoryExist.Name + ") successfully.";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Fail to change status category (" + categoryExist.Name + ")!";
					return result;
				}
			}
			var deleteResult = await _unitOfWork.CategoryRepository.DeleteAsync(id.ToString());
			if ( deleteResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Delete catgory (" + categoryExist.Name + ") successfully.";
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Fail to delete category (" + categoryExist.Name + ")!";
				return result;
			}
		}
		#endregion
		private bool CheckMatchType(string type)//nhan noi dung cua type tu request, do theo list type cua category, neu khong nam trong list thi bao loi
		{
			foreach ( var item in categoryTypeList )
			{
				if ( item.Equals(type) )
				{
					return true;//nghia la co trung voi 1 type trong nay
				}
			}
			return false;
		}


	}
}
