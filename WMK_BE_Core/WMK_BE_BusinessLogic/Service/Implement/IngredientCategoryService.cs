using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class IngredientCategoryService : IIngredientCategoryService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly CreateIngredientCategoryValdator _createValidator;//dung cho validate ham create. nam trong IngredientValidator
		private readonly FullIngredientCategoryValdator _fullValidator;//dung cho validate cac ham update. nam trong IngredientValidator
		public IngredientCategoryService(IMapper mapper , IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_createValidator = new CreateIngredientCategoryValdator();
			_fullValidator = new FullIngredientCategoryValdator();
		}
		#region get all
		public async Task<ResponseObject<List<IngredientCategoryResponse>>> GetAllAsync(GetAllIngredientCategoriesRequest? model)
		{
			var result = new ResponseObject<List<IngredientCategoryResponse>>();
			var ingredientCategories = new List<IngredientCategory>();
			var ingredientCategoriesResponse = new List<IngredientCategoryResponse>();
			if ( model != null && !model.Name.IsNullOrEmpty() )
			{
				var ingredientCategoriesByname = await GetByNameAsync(model.Name);
				if ( ingredientCategoriesByname != null && ingredientCategoriesByname.Data != null )
				{
					ingredientCategoriesResponse.AddRange(ingredientCategoriesByname.Data);
				}
			}
			else
			{
				ingredientCategories = await _unitOfWork.IngredientCategoryRepository.GetAllAsync();
				ingredientCategoriesResponse = _mapper.Map<List<IngredientCategoryResponse>>(ingredientCategories);
			}
			result.StatusCode = 200;
			result.Message = "Danh sách Ingredeint category: (" + ingredientCategoriesResponse.Count + ")";
			result.Data = ingredientCategoriesResponse ?? [];
			return result;
		}
		public async Task<ResponseObject<List<IngredientCategoryResponse>>> GetByNameAsync(string name)
		{
			var result = new ResponseObject<List<IngredientCategoryResponse>>();
			var currentList = await _unitOfWork.IngredientCategoryRepository.GetAllAsync();
			if ( currentList != null && currentList.Any() )
			{
				var foundList = currentList.Where(x => x.Name.RemoveDiacritics().ToLower().Contains(name.RemoveDiacritics().ToLower())).ToList();
				if ( !foundList.Any() )
				{
					result.StatusCode = 404;
					result.Message = "Không tìm thấy Ingredient category chứa từ khóa: " + name;
					return result;
				}
				else
				{
					result.StatusCode = 200;
					result.Message = "Ingredient category chứa tù7 khóa: " + name;
					result.Data = _mapper.Map<List<IngredientCategoryResponse>>(foundList);
				}
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Không có dữ liệu";
				return result;
			}
			return result;
		}
		#endregion

		#region create new ingredient category
		public async Task<ResponseObject<IngredientCategoryResponse>> CreateNew(CreateIngredientCategoryRequest request)
		{
			var result = new ResponseObject<IngredientCategoryResponse>();
			var validateResult = _createValidator.Validate(request);
			if ( !validateResult.IsValid )//kiem tra request co du lieu khong
			{
				var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 500;
				result.Message = string.Join(" - " , error);
				return result;
			}
			var currentList = await _unitOfWork.IngredientCategoryRepository.GetAllAsync();
			var found = currentList.FirstOrDefault(x => x.Name.ToLower().Equals(request.Name.ToLower())); //tim trung ten
			if ( found != null && found.Status.ToString().Equals("Available") )
			{
				result.StatusCode = 500;
				result.Message = "Trùng tên!";
				return result;
			}
			IngredientCategory newOne = _mapper.Map<IngredientCategory>(request);
			var createResult = await _unitOfWork.IngredientCategoryRepository.CreateAsync(newOne);
			if ( createResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Tạo thành công"; //kiem tra voi cach khac, dung ham Create1, sau do cho tim ingredient voi name vua tao, nau tim thay thi lay Id tra ve, ko tim thay nghia la loi --> bao loi
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Tạo không thành công!";
				return result;
			}
		}
		#endregion

		#region update ingredient category
		public async Task<ResponseObject<IngredientCategoryResponse>> UpdateCategory(Guid id , FullIngredientCategoryRequest request)
		{
			var result = new ResponseObject<IngredientCategoryResponse>();
			//validate request dua vao
			var validateResult = _fullValidator.Validate(request);
			if ( !validateResult.IsValid )//kiem tra request
			{
				var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			//check co ton tai trong db khong
			var found = await _unitOfWork.IngredientCategoryRepository.GetByIdAsync(id.ToString()); //tim trung ten
			if ( found != null )
			{
				var checkDuplicateName = _unitOfWork.IngredientCategoryRepository.Get(x => x.Name.Trim().ToLower().Equals(request.Name.Trim().ToLower())
																						&& x.Id != found.Id).FirstOrDefault();
				if ( checkDuplicateName != null )
				{
					result.StatusCode = 500;
					result.Message = "Trùng tên";
					return result;
				}
				//detach entity if need
				//_unitOfWork.IngredientCategoryRepository.DetachEntity(found);

				//found = _mapper.Map<IngredientCategory>(request);
				_mapper.Map(request , found);
				var updateResult = await _unitOfWork.IngredientCategoryRepository.UpdateAsync(found);
				if ( updateResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Cập nhật ingredient category thành công";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Cập nhật ingredietn category không thành công";
					result.Data = _mapper.Map<IngredientCategoryResponse>(found);
					return result;
				}
			}
			else
			{
				await DeleteById(id);
				result.StatusCode = 404;
				result.Message = "Không tìm thấy ingredient category!";
				return result;
			}
		}
		#endregion

		#region Delete by id (xoa luon khoi db)
		public async Task<ResponseObject<IngredientCategoryResponse>> DeleteById(Guid request)
		{
			var result = new ResponseObject<IngredientCategoryResponse>();
			var found = await _unitOfWork.IngredientCategoryRepository.GetByIdAsync(request.ToString());
			//_unitOfWork.IngredientCategoryRepository.DetachEntity(found);
			if ( found == null )
			{
				result.StatusCode = 404;
				result.Message = "Không tìm thấy ingredient category!";
				return result;
			}
			var deleteResult = await _unitOfWork.IngredientCategoryRepository.DeleteAsync(request.ToString());
			if ( deleteResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Xóa thành công";
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Xóa không thành công!";
				return result;
			}
		}
		#endregion

		#region Get by id
		public async Task<ResponseObject<IngredientCategoryResponse>> GetById(Guid request)
		{
			var result = new ResponseObject<IngredientCategoryResponse>();
			IngredientCategory? found = await _unitOfWork.IngredientCategoryRepository.GetByIdAsync(request.ToString());
			if ( found == null )
			{
				result.StatusCode = 404;
				result.Message = "Không tìm thấy bản ghi";
				return result;
			}
			result.StatusCode = 200;
			result.Message = "Tìm thấy";
			result.Data = _mapper.Map<IngredientCategoryResponse>(found);
			return result;
		}

		#endregion

		#region Change status
		public async Task<ResponseObject<IngredientCategoryResponse>> ChangeStatusIngredientCategoryAsync(Guid id , ChangeStatusIngredientCategoryRequest request)
		{
			var result = new ResponseObject<IngredientCategoryResponse>();

			//check exist
			var ingredientCategoryExist = await _unitOfWork.IngredientCategoryRepository.GetByIdAsync(id.ToString());

			if ( ingredientCategoryExist != null )
			{
				ingredientCategoryExist.Status = request.Status;
				var changeStatusResult = await _unitOfWork.IngredientCategoryRepository.UpdateAsync(ingredientCategoryExist);
				if ( changeStatusResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Cập nhật thành công";
					return result;
				}
				result.StatusCode = 500;
				result.Message = "Cập nhật thành công";
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Ingredient category không tồn tại!";
			return result;
		}
		#endregion
	}
}
