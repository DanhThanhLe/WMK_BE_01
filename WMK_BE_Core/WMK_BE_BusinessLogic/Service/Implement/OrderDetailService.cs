using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.CustomPlanModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRecipeIngredientOrderDetailService _recipeIngredientOrderDetailService;

        public OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper, IRecipeIngredientOrderDetailService recipeIngredientOrderDetailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _recipeIngredientOrderDetailService = recipeIngredientOrderDetailService;
        }



        #region create 
        public async Task<ResponseObject<List<OrderDetail>?>> CreateOrderDetailAsync(Guid orderId, List<CreateOrderDetailRequest> RecipeList)
        {
            //kiêm tra thông tin order (getId từ orderId)
            //kiểm tra thông tin recipeList truyền vào 
            //-> tính coi có đủ suất ăn quy định ko (5-100)
            var result = new ResponseObject<List<OrderDetail>?>();
            var orderFound = await _unitOfWork.OrderRepository.GetByIdAsync(orderId.ToString());
            if (orderFound == null)
            {
                result.StatusCode = 500;
                result.Message = "Not found Order Id. Say from CreateCustomPlanAsync - CustomPlanService";
                return result;
            }
            if (RecipeList.Count() > 0)//trong list co thong tin
            {
                OrderDetail newOne;
                List<OrderDetail> returnList = new List<OrderDetail>();
                foreach (var item in RecipeList)
                {
                    Recipe? checkRecipe;
                    checkRecipe = await _unitOfWork.RecipeRepository.GetByIdAsync(item.RecipeId.ToString());
                    if (checkRecipe != null && checkRecipe.ProcessStatus == ProcessStatus.Approved)//check coi recipe tim duoc co dang cho dat hang hay khong
                    {
                        newOne = _mapper.Map<OrderDetail>(item);
                        newOne.OrderId = orderId;
                        //newOne.StandardWeeklyPlanId = Guid.Empty;
                        var createResult = await _unitOfWork.OrderDetailRepository.CreateAsync(newOne);//bar dau tao thong tin luu tru cho ingredient dua vao reipce
                        if (!createResult)
                        {
                            result.StatusCode = 500;
                            result.Message = "Error at CreateCustomPlanAsync - CustomPlanService ";
                            return result;
                        }
                        //bat dau tao thong tin recipeIngredient trong orderDetail
                        await _unitOfWork.CompleteAsync();
                        var createRecipeIngredientOrderDetail = await _recipeIngredientOrderDetailService.CreateRecipeIngredientOrderDetail(newOne.Id, item.RecipeId, item.Quantity);
                        if (createRecipeIngredientOrderDetail.StatusCode != 200)//ko tao dc recipeIngredient trong order detail
                        {
                            await _unitOfWork.OrderDetailRepository.DeleteAsync(newOne.Id.ToString());
                            await _unitOfWork.CompleteAsync();
                            result.StatusCode = createRecipeIngredientOrderDetail.StatusCode;
                            result.Message = createRecipeIngredientOrderDetail.Message;
                            return result;
                        }
                        returnList.Add(newOne);
                    }//check coi recipe tim duoc co dang cho dat hang hay khong
                    else
                    {
                        result.StatusCode = 500;
                        result.Message = "Error at CreateCustomPlanAsync - CustomPlanService. Unavailable recipe ";
                        return result;
                    }
                }
                result.StatusCode = 200;
                result.Message = "OK - Create OrderDetail ok ";
                result.Data = _mapper.Map<List<OrderDetail>>(returnList);
                return result;
            }
            else//khong co thong tin
            {
                result.StatusCode = 500;
                result.Message = "Vui lòng đặt ít nhất 5 món ăn";
                return result;
            }
        }
        #endregion


    }
}
