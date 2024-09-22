using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeStepModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeStepModel.RecipeStep;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeStepModel.RecipeStep;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class RecipeStepService : IRecipeStepService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public RecipeStepService(IUnitOfWork unitOfWork , IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		#region Get all
		public async Task<ResponseObject<List<RecipeStepRespone>>> GetRecipeSteps()
		{
			var result = new ResponseObject<List<RecipeStepRespone>>();
			var recipeStepList = await _unitOfWork.RecipeStepRepository.GetAllAsync();
			if ( recipeStepList != null && recipeStepList.Count > 0 )
			{
				result.StatusCode = 200;
				result.Message = "Tìm thấy";
				result.Data = _mapper.Map<List<RecipeStepRespone>>(recipeStepList);
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Không tìm thấy công thức";
				return result;
			}
		}
		#endregion

		#region Get-by-recipe-id
		public async Task<ResponseObject<List<RecipeStepRespone>>> GetByRecipeId(Guid recipeId)
		{
			var result = new ResponseObject<List<RecipeStepRespone>>();
			var currentList = await _unitOfWork.RecipeStepRepository.GetAllAsync();
			if ( currentList.Count == 0 )
			{
				result.StatusCode = 400;
				result.Message = "Không tìm thấy dữ liệu";
				return result;
			}
			List<RecipeStep> foundList = new List<RecipeStep>();
			foreach ( var recipeStep in currentList )
			{
				if ( recipeStep.RecipeId == recipeId )
				{
					foundList.Add(recipeStep);
				}
			}
			if ( foundList.Count == 0 )
			{
				result.StatusCode = 400;
				result.Message = "Không tim thấy dữ liệu";
				return result;
			}
			result.StatusCode = 200;
			result.Message = "Tìm thấy";
			result.Data = _mapper.Map<List<RecipeStepRespone>>(foundList);
			return result;
		}
		#endregion Get-by-recipe-id

		#region Create-multiple-steps-for-new-recipe
		public async Task<ResponseObject<List<RecipeStep>>> CreateRecipeSteps(Guid recipeId , List<CreateRecipeStepRequest> stepList)
		{
			var result = new ResponseObject<List<RecipeStep>>();
			/*
             xac nhan recipe
            chay vong lap tao recipe step
            -check trung lap
            -check index
             */
			if ( stepList.Count() <= 0 )
			{
				result.StatusCode = 400;
				result.Message = "Không có thông tin các bước chế biến";
				return result;
			}
			var foundRecipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
			if ( foundRecipe == null )
			{
				result.StatusCode = 400;
				result.Message = "Không tìm thấy dữ liệu";
				return result;
			}
			RecipeStep newStep = new RecipeStep();
			List<RecipeStep> returnList = new List<RecipeStep>();
			newStep.RecipeId = recipeId;
			foreach ( var step in stepList )
			{
				newStep = _mapper.Map<RecipeStep>(step);
				newStep.RecipeId = recipeId;
				var createResult = await _unitOfWork.RecipeStepRepository.CreateAsync(newStep);
				if ( !createResult )//cho nay tra ve luon thong tin cu the type nao bi tao loi
				{
					result.StatusCode = 500;
					result.Message = "Tạo mới bước chế biến không thành công";
					result.Data = null;
					return result;
				}
				await _unitOfWork.CompleteAsync();
				returnList.Add(newStep);
			}
			result.StatusCode = 200;
			result.Message = "Tạo mới thành công";
			result.Data = returnList;
			return result;
		}
		#endregion

		#region Delete
		public async Task<ResponseObject<RecipeStep>> DeleteAsync(Guid recipeStepId)
		{
			var result = new ResponseObject<RecipeStep>();
			var deleteResult = await _unitOfWork.RecipeStepRepository.DeleteAsync(recipeStepId.ToString());
			if ( deleteResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Xoá thành công.";
				return result;
			}
			result.StatusCode = 500;
			result.Message = "Xóa không thành công";
			return result;
		}

		public async Task<ResponseObject<List<RecipeStep>>> DeleteRecipeStepsByRecipe(Guid recipeId)
		{
			var result = new ResponseObject<List<RecipeStep>>();
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
				//take recipeSteps asign to recipe
				var recipeSteps = _unitOfWork.RecipeStepRepository.GetAll()
					.Where(rc => rc.RecipeId == recipeId).ToList();
				//if ( recipeSteps == null || !recipeSteps.Any() )
				//{
				//	result.StatusCode = 404;
				//	result.Message = "No recipeSteps found for the given Recipe.";
				//	return result;
				//}
				// Nếu dữ liệu truyền vô trùng id thì không xóa mà update
				_unitOfWork.RecipeStepRepository.DeleteRange(recipeSteps);
				await _unitOfWork.CompleteAsync();

				result.StatusCode = 200;
				result.Message = "Xóa thàn công";
				result.Data = recipeSteps.ToList();
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
			}

			return result;
		}
		#endregion

		#region Update
		public async Task<ResponseObject<List<RecipeStep>>> UpdateRecipeStepsByRecipe(Guid Id , CreateRecipeStepRequest model)
		{
			var result = new ResponseObject<List<RecipeStep>>();
			//check recipe step exist
			var recipeStepExist = await _unitOfWork.RecipeStepRepository.GetByIdAsync(Id.ToString());
			if ( recipeStepExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Khong tìm thấy dữ liệu";
				return result;
			}

			_mapper.Map(model , recipeStepExist);
			var updateResult = await _unitOfWork.RecipeStepRepository.UpdateAsync(recipeStepExist);
			if ( updateResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Cập nhật thành công";
				return result;
			}
			result.StatusCode = 500;
			result.Message = "Cập nhật thất bại";
			return result;
		}
		#endregion
	}
}
