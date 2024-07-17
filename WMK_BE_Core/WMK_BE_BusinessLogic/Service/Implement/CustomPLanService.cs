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

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class CustomPLanService : ICustomPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomPLanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        #region create (tạo ra khi order của khách ko nhận id của weekly plan có sẵn mà là custome plan của khách)
        public async Task<ResponseObject<List<CustomPlanResponse>?>> CreateCustomPlanAsync(Guid orderId, List<CreateCustomPlanRequest> RecipeList)
        {
            //kiêm tra thông tin order (getId từ orderId)
            //kiểm tra thông tin recipeList truyền vào 
            //-> tính coi có đủ suất ăn quy định ko (5-100)
            var result = new ResponseObject<List<CustomPlanResponse>?>();
            var orderFound = await _unitOfWork.OrderRepository.GetByIdAsync(orderId.ToString());
            if (orderFound == null)
            {
                result.StatusCode = 500;
                result.Message = "Not found Order Id. Say from CreateCustomPlanAsync - CustomPlanService";
                return result;
            }
            if (RecipeList.Count() > 0)//trong list co thong tin
            {
                int quantity = 0;
                foreach (var item in RecipeList)
                {
                    quantity += item.Quantity;
                }
                if (quantity < 5)//khong du so luong phan an toi thieu
                {
                    result.StatusCode = 500;
                    result.Message = "Vui lòng đặt ít nhất 5 món ăn hoặc 5 phần ăn";
                    return result;
                }

                CustomPlan newOne;
                foreach (var item in RecipeList)
                {
                    newOne = _mapper.Map<CustomPlan>(item);
                    newOne.OrderId = orderId;
                    var createResult = await _unitOfWork.CustomPlanRepository.CreateAsync(newOne);
                    if (!createResult)
                    {
                        result.StatusCode = 500;
                        result.Message = "Error at CreateCustomPlanAsync - CustomPlanService ";
                        return result;
                    }
                    result.StatusCode = 500;
                    result.Message = "Error at CreateCustomPlanAsync - CustomPlanService ";
                    return result;
                }
            }
            else//khong co thong tin
            {
                result.StatusCode = 500;
                result.Message = "Vui lòng đặt ít nhất 5 món ăn";
                return result;
            }
            throw new NotImplementedException();
        }
        #endregion


    }
}
