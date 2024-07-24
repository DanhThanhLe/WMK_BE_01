using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeIngredientDetailModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class RecipeIngredientOrderDetailService : IRecipeIngredientOrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RecipeIngredientOrderDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseObject<List<RecipeIngredientOrderDetail>>> CreateRecipeIngredientOrderDetail(Guid orderDetailId, Guid recipeId, int recipeQuantity)
        {
            var result = new ResponseObject<List<RecipeIngredientOrderDetail>>();
            try
            {
                //kiem tra id orderDetailId
                //kiem tra recipeId trong orderId
                //lay thong tin cua recipe
                //lay thong tin cua recipeIngredient
                //lay thong tin cua ingreident
                //nhan thong tin gia voi thong tin cua amount trong recipe amount
                OrderDetail checkOrderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(orderDetailId.ToString());
                if(checkOrderDetail.Id.ToString() == null)
                {
                    result.StatusCode = 500;
                    result.Message = "Order Detail not exist";
                    return result;
                }
                else
                {
                    List<RecipeIngredientOrderDetail> returnList = new List<RecipeIngredientOrderDetail>();
                    if (recipeId.ToString().ToLower().Equals(checkOrderDetail.RecipeId.ToString().ToLower()))
                    {
                        var checkRecipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());//tim recipe
                        if (checkRecipe.Id.ToString() != null)//bat dau tao thong tin recipeIngredient trong 
                        {
                            //lay thong tin cua recipeIngredient
                            //lay thong tin cua ingreident
                            //nhan thong tin gia voi thong tin cua amount trong recipe amount
                            RecipeIngredientOrderDetail newOne = new RecipeIngredientOrderDetail();
                            newOne.OrderDetailId = orderDetailId;
                            newOne.RecipeId = recipeId;
                            foreach(var item in checkRecipe.RecipeIngredients)
                            {
                                newOne.IngredientId = item.IngredientId;
                                newOne.Amount = item.Amount;
                                newOne.IngredientPrice = item.Amount * item.Ingredient.Price * recipeQuantity;//dinh luong trong recipeIngredient * gia unit cua Ingredient * so luong cua so phan recipe
                                //var createResult = await _unitOfWork.RecipeIngredientOrderDetailRepository.CreateAsync(newOne);
                                //if (!createResult)
                                //{
                                //    result.StatusCode = 500;
                                //    result.Message = "Error when create RecipeIngredient Order Detail";
                                //    return result;
                                //}
                                //await _unitOfWork.CompleteAsync();
                                returnList.Add(newOne);
                            }
                            if (returnList.Any())
                            {
                                await _unitOfWork.RecipeIngredientOrderDetailRepository.AddRangeAsync(returnList);
                                
                            }
                            await _unitOfWork.CompleteAsync();
                            result.StatusCode = 200;
                            result.Message = "OK";
                            result.Data = returnList;
                            return result;
                        }
                        else
                        {
                            result.StatusCode = 500;
                            result.Message = "Recipe not found";
                            return result;
                        }
                    }
                    else//recipe id dua xuong ko khop voi recipe id duoc luu trong order detail
                    {
                        result.StatusCode=500;
                        result.Message = "Recipe Id not match with request";
                        return result;
                    }
                }
            }catch (Exception ex)
            {
                result.StatusCode = 500;
                result.Message = ex.Message;
                return result;
            }
        }
    }
}
