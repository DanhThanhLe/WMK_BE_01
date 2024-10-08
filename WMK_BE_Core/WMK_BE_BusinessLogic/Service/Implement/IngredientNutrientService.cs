﻿using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientNutrientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientNutrientModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class IngredientNutrientService : IIngredientNutrientService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly CreateIngredientNutrientValidator _createIngredientNutrientValidator;
		private readonly FullIngredientNutrientValidator _fullIngredientNutrientValidator;
		public IngredientNutrientService(IUnitOfWork unitOfWork , IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_createIngredientNutrientValidator = new CreateIngredientNutrientValidator();
			_fullIngredientNutrientValidator = new FullIngredientNutrientValidator();
		}

		private async Task<List<IngredientNutrient>> GetAllToProcess()
		{
			var currentList = await _unitOfWork.IngredientNutrientRepository.GetAllAsync();
			if ( currentList.Any() )
			{
				return currentList;
			}
			return new List<IngredientNutrient>();
		}

		#region get all
		public async Task<ResponseObject<List<IngredientNutrientResponse>>> GetAll()
		{
			var result = new ResponseObject<List<IngredientNutrientResponse>>();
			List<IngredientNutrient> currentList = await GetAllToProcess();
			if ( currentList.Count > 0 )
			{
				result.StatusCode = 200;
				result.Message = "Danh sách Ingrdient nutrient:";
				result.Data = _mapper.Map<List<IngredientNutrientResponse>>(currentList);
				return result;
			}
			result.StatusCode = 500;
			result.Message = "Không có dữ liệu";
			return result;
		}
		#endregion

		#region get by ingredient id
		public async Task<ResponseObject<IngredientNutrientResponse>> GetByIngredientId(Guid request)
		{
			var result = new ResponseObject<IngredientNutrientResponse>();
			var currentList = await _unitOfWork.IngredientNutrientRepository.GetAllAsync();
			if ( currentList.Count > 0 )
			{
				IngredientNutrient foundResult = currentList.FirstOrDefault(x => x.IngredientID.ToString().Equals(request.ToString()));
				if ( foundResult != null )
				{
					result.StatusCode = 200;
					result.Message = "Tìm thấy";
					result.Data = _mapper.Map<IngredientNutrientResponse>(foundResult);
					return result;
				}
				else
				{
					result.StatusCode = 400;
					result.Message = "Không tìm thấy";
					return result;
				}
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Dữ liệu trống";
				return result;
			}
		}
		#endregion

		#region get by id
		public async Task<ResponseObject<IngredientNutrientResponse>> GetById(Guid request)
		{
			var result = new ResponseObject<IngredientNutrientResponse>();
			var foundResult = await _unitOfWork.IngredientNutrientRepository.GetByIdAsync(request.ToString());
			if ( foundResult != null )
			{
				result.StatusCode = 200;
				result.Message = "Tìm thấy";
				result.Data = _mapper.Map<IngredientNutrientResponse>(foundResult);
				return result;
			}
			else
			{
				result.StatusCode = 400;
				result.Message = "Không tìm thấy";
				return result;
			}
		}
		#endregion

		#region Create
		public async Task<ResponseObject<IngredientNutrient>> Create(Guid IngredientID , CreateIngredientNutrientRequest request)
		{
			var result = new ResponseObject<IngredientNutrient>();
			var validateResult = _createIngredientNutrientValidator.Validate(request);
			if ( !validateResult.IsValid )//bat loi dau vao
			{
				var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 500;
				result.Message = string.Join(" - /n" , error);
				return result;
			}
			else//du lieu cua request ko co loi
			{
				var checkIngredient = await _unitOfWork.IngredientRepository.GetByIdAsync(IngredientID.ToString());
				var currentList = await GetAllToProcess();
				var checkExist = currentList.FirstOrDefault(x => x.IngredientID == IngredientID);
				if ( checkIngredient != null && checkExist != null )//khac null nghia la co ton tai ingredient nhu tren, tim luon co thong tin nutrient chua
				{
					result.StatusCode = 400;
					result.Message = "Đã tồn tại thông tin dinh dưỡng cho nguyên liệu đang xử lí";
					return result;

				}
				else if ( checkIngredient != null && checkExist == null )//nghia la co ingrerdient ton tai va chua co thong tin nutrient
				{
					IngredientNutrient newOne = _mapper.Map<IngredientNutrient>(request);
					newOne.IngredientID = IngredientID;
					var createResult = await _unitOfWork.IngredientNutrientRepository.CreateAsync(newOne);
					if ( createResult )
					{
						await _unitOfWork.CompleteAsync();
						//checkIngredient.IngredientNutrient = newOne;
						result.StatusCode = 200;
						result.Message = "Tạo thành công";
						result.Data = _mapper.Map<IngredientNutrient>(newOne);
						return result;
					}
					else
					{
						result.StatusCode = 500;
						result.Message = "Tạo thất bại";
						return result;
					}

				}
				else if ( checkIngredient == null )//null la coi nhu ko tim thay ingredient trong db -> bao loi khog tim thay
				{
					result.StatusCode = 400;
					result.Message = "Nguyên liệu không tồn tại";
					return result;
				}
				else//cac loai ket qua khac
				{
					result.StatusCode = 500;
					result.Message = "Lỗi bất định khi đang tạo thông tin dinh dưỡng";
					return result;
				}
			}
		}
		#endregion

		#region Update
		public async Task<ResponseObject<IngredientNutrientResponse>> Update(Guid idIngredientNutrient , IngredientNutrientRequest request)
		{
			var result = new ResponseObject<IngredientNutrientResponse>();
			try
			{
				//kiem tra voi ingredient id xem
				//+ co ton tai ingredient ko
				//+ co ton tai nutrient nao chua, phong bi duplicate
				var ingredientNutrientExist = await _unitOfWork.IngredientNutrientRepository.GetByIdAsync(idIngredientNutrient.ToString());
				if ( ingredientNutrientExist == null )
				{
					result.StatusCode = 404;
					result.Message = "Không tìm thấy bản ghi";
					return result;
				}
				_mapper.Map(request , ingredientNutrientExist);
				var updateIngredientNutrientResult = await _unitOfWork.IngredientNutrientRepository.UpdateAsync(ingredientNutrientExist);
				if ( !updateIngredientNutrientResult )
				{
					result.StatusCode = 500;
					result.Message = "Cập nhật không thành công";
					return result;
				}
				//await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Cập nhật thành công";
				return result;
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
				return result;
			}

		}
		#endregion

		#region delete
		public async Task<ResponseObject<IngredientNutrientResponse>> DeleteById(Guid request)
		{
			var result = new ResponseObject<IngredientNutrientResponse>();
			var checkExist = await _unitOfWork.IngredientNutrientRepository.GetByIdAsync(request.ToString());
			if ( checkExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Không tìm thấy bản ghi";
				return result;
			}
			else
			{
				var deleteResult = await _unitOfWork.IngredientNutrientRepository.DeleteAsync(request.ToString());
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
					result.Message = "Xóa không thành công";
					return result;
				}
			}
		}
		#endregion

	}
}

