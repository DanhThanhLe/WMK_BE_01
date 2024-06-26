﻿using AutoMapper;
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

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<ResponseObject<CategoryResponseModel>> GetAllAsync()
        {
            var result = new ResponseObject<CategoryResponseModel>();
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            if (categories != null && categories.Count > 0)
            {
                result.StatusCode = 200;
                result.Message = "Categories";
                result.List = _mapper.Map<List<CategoryResponseModel>>(categories);
                return result;
            }
            else
            {
                result.StatusCode = 404;
                result.Message = "Don't have Categories!";
                return result;
            }
        }
        public async Task<ResponseObject<CategoryResponseModel?>> GetByIdAsync(Guid id)
        {
            var result = new ResponseObject<CategoryResponseModel?>();

            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id.ToString());
            if (category != null)
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
        public async Task<ResponseObject<CategoryResponseModel>> CreateCategoryAsync(CreateCategoryRequestModel model)
        {
            var result = new ResponseObject<CategoryResponseModel>();
            var newCategory = _mapper.Map<Category>(model);
            //check co type trong quy dinh
            var checkMatchTypeResult = CheckMatchType(newCategory.Type);
            if (!checkMatchTypeResult)
            {
                result.StatusCode = 400;
                result.Message = "Not match pre-set category types!";
                return result;
            }
            //check trung ten
            var currentList = await _unitOfWork.CategoryRepository.GetAllAsync();
            var checkDuplicateNameResult = currentList.FirstOrDefault(x => x.Name == newCategory.Name);
            if (checkDuplicateNameResult != null)
            {
                result.StatusCode = 400;
                result.Message = "Duplicate name with category id: " + checkDuplicateNameResult.Id + " !";
                return result;
            }
            newCategory.Status = BaseStatus.Available;
            var createResult = await _unitOfWork.CategoryRepository.CreateAsync(newCategory);
            if (createResult)
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
        public async Task<ResponseObject<CategoryResponseModel>> UpdateCategoryAsync(UpdateCategoryRequestModel model)
        {
            var result = new ResponseObject<CategoryResponseModel>();

            var categoryExist = await _unitOfWork.CategoryRepository.GetByIdAsync(model.Id.ToString());

            if (categoryExist == null)
            {
                result.StatusCode = 200;
                result.Message = "Category not exist!";
                return result;
            }
            if (!string.IsNullOrEmpty(model.Name))
            {
                categoryExist.Name = model.Name;
            }
            if (!string.IsNullOrEmpty(model.Description))
            {
                categoryExist.Description = model.Description;
            }
            if (model.Status != null)
            {
                categoryExist.Status = model.Status ?? 0;
            }
            var updateResult = await _unitOfWork.CategoryRepository.UpdateAsync(categoryExist);
            if (updateResult)
            {
                await _unitOfWork.CompleteAsync();
                result.StatusCode = 200;
                result.Message = "Update category (" + categoryExist.Name + ") successfully.";
                return result;
            }
            else
            {
                result.StatusCode = 500;
                result.Message = "Update category unsuccessfully!";
                return result;
            }

        }
        public async Task<ResponseObject<CategoryResponseModel>> DeleteCategoryAsync(DeleteCategoryRequestModel model)
        {
            var result = new ResponseObject<CategoryResponseModel>();
            var categoryExist = await _unitOfWork.CategoryRepository.GetByIdAsync(model.Id.ToString());
            if (categoryExist == null)
            {
                result.StatusCode = 500;
                result.Message = "Category not found!";
                return result;
            }
            var recipeExistCategory = await _unitOfWork.CategoryRepository.RecipeExistCategoryAsync(model.Id);
            if (recipeExistCategory)
            {
                categoryExist.Status = WMK_BE_RecipesAndPlans_DataAccess.Enums.BaseStatus.UnAvailable;
                var updateResult = await _unitOfWork.CategoryRepository.UpdateAsync(categoryExist);
                if (updateResult)
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
            var deleteResult = await _unitOfWork.CategoryRepository.DeleteAsync(model.Id.ToString());
            if (deleteResult)
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
        List<string> categoryTypeList = new List<string> { "Nation", "Classify", "Cooking Method", "Meal in day" };

        private bool CheckMatchType(string type)//nhan noi dung cua type tu request, do theo list type cua category, neu khong nam trong list thi bao loi
        {
            foreach (var item in categoryTypeList)
            {
                if (item.Equals(type))
                {
                    return true;//nghia la co trung voi 1 type trong nay
                }
            }
            return false;
        }

        public async Task<ResponseObject<CategoryResponseModel>> GetCategoryByType(string type)
        {
            var result = new ResponseObject<CategoryResponseModel>();
            var checkMatchTypeResult = CheckMatchType(type);
            if (!checkMatchTypeResult)
            {
                result.StatusCode = 400;
                result.Message = "Not match pre-set category types!";
                return result;
            }
            var currentList = await _unitOfWork.CategoryRepository.GetAllAsync();
            List<Category> foundList = new List<Category>();
            foreach (var item in currentList)
            {
                if (item.Type.ToLower().Equals(type.ToLower()) && item.Status == BaseStatus.Available)
                {
                    foundList.Add(item);
                }
            }
            result.StatusCode = 200;
            result.Message = "List category with " + type + " type";
            result.List = _mapper.Map<List<CategoryResponseModel>>(foundList);
            return result;

        }

        public async Task<ResponseObject<CategoryResponseModel>> GetcategoryByName(string name)
        {
            var result = new ResponseObject<CategoryResponseModel>();
            var currentList = await _unitOfWork.CategoryRepository.GetAllAsync();
            List<Category> foundList = new List<Category>();
            foreach (var item in currentList)
            {
                if(item.Name.ToLower().Contains(name.ToLower()) && item.Status == BaseStatus.Available)
                {
                    foundList.Add(item);
                }
            }
            if(foundList.Count == 0)
            {
                result.StatusCode = 400;
                result.Message = "Not found with name contains " + name;
                return result;
            }
            result.StatusCode = 200;
            result.Message = "List category with name contains " + name;
            result.List = _mapper.Map<List<CategoryResponseModel>>(foundList);
            return result;
        }
    }
}
