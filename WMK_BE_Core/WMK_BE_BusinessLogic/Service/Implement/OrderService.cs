using AutoMapper;
using Azure.Core;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITransactionService _transactionService;
        #region Validator
        private readonly CreateOrderModelValidator _createOrderValidator;
        private readonly UpdateOrderModelValidator _updateOrderValidator;
        private readonly UpdateOrderByUserModelValidator _updateOrderByUserValidator;
        private readonly IOrderDetailService _orderDetailService;
        #endregion
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IOrderDetailService orderDetailService, ITransactionService transactionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _transactionService = transactionService;
            _createOrderValidator = new CreateOrderModelValidator();
            _updateOrderValidator = new UpdateOrderModelValidator();
            _updateOrderByUserValidator = new UpdateOrderByUserModelValidator();
            _orderDetailService = orderDetailService;
        }
        #region get all
        public async Task<ResponseObject<List<OrderResponse>>> GetAllOrdersAsync(GetAllOrdersRequest? model)
        {
            var result = new ResponseObject<List<OrderResponse>>();

            var orders = new List<Order>();
            var ordersResponse = new List<OrderResponse>();
            if (model != null && (!model.ReceiveName.IsNullOrEmpty() || !model.OrderCode.IsNullOrEmpty()))
            {
                if (!model.ReceiveName.IsNullOrEmpty())
                {
                    var ordersByReciveName = await GetOrdersByReciveName(model.ReceiveName);
                    if (ordersByReciveName != null && ordersByReciveName.Data != null)
                    {
                        ordersResponse.AddRange(ordersByReciveName.Data);
                    }
                }
                if (!model.OrderCode.IsNullOrEmpty())
                {
                    var ordersByOrderCode = await GetOrdersByOrderCode(model.OrderCode);
                    if (ordersByOrderCode != null && ordersByOrderCode.Data != null)
                    {
                        ordersResponse.AddRange(ordersByOrderCode.Data);
                    }
                }
                // Loại bỏ các phần tử trùng lặp dựa trên Id
                ordersResponse = ordersResponse
                    .GroupBy(c => c.Id)
                    .Select(g => g.First())
                    .ToList();
            }
            else
            {
                orders = await _unitOfWork.OrderRepository.GetAllAsync();
                ordersResponse = _mapper.Map<List<OrderResponse>>(orders);
            }
            if (ordersResponse != null && ordersResponse.Any())
            {

                foreach (var item in ordersResponse)
                {
                    var user = await _unitOfWork.UserRepository.GetByIdAsync(item.UserId.ToString());
                    if (user != null)
                    {
                        item.UserId = user.FirstName + " " + user.LastName;
                    }
                }
                result.StatusCode = 200;
                result.Message = "Get order list success (" + ordersResponse.Count() + ")";
                //var ordersResp = ;
                //foreach ( var order in ordersResp )
                //{
                //	if ( order.FeedBacks != null && order.FeedBacks.CreatedBy != null )
                //	{
                //		var createBy = _unitOfWork.UserRepository.GetById(order.FeedBacks.CreatedBy);
                //		if ( createBy != null )
                //		{
                //			order.FeedBacks.CreatedBy = createBy.UserName ?? order.FeedBacks.CreatedBy;
                //		}
                //	}
                //}
                result.Data = ordersResponse.OrderBy(o => o.OrderDate).ToList();
                return result;
            }
            else
            {
                result.StatusCode = 404;
                result.Message = "Don't have Order!";
                result.Data = [];
                return result;
            }
        }

        public async Task<ResponseObject<List<OrderResponse>>> GetOrdersByReciveName(string reciveName)
        {
            var result = new ResponseObject<List<OrderResponse>>();

            var orders = await _unitOfWork.OrderRepository.GetAllAsync();

            orders = orders.Where(o => o.ReceiveName.ToLower().RemoveDiacritics().Contains(reciveName.ToLower().RemoveDiacritics())).ToList();
            if (orders != null && orders.Any())
            {
                result.StatusCode = 200;
                result.Message = "Order list found by recive name success";
                result.Data = _mapper.Map<List<OrderResponse>>(orders);
                return result;
            }
            result.StatusCode = 404;
            result.Message = "Order list not found by recive name: " + reciveName;
            return result;

        }
        public async Task<ResponseObject<List<OrderResponse>>> GetOrdersByOrderCode(string orderCode)
        {
            var result = new ResponseObject<List<OrderResponse>>();

            var orders = await _unitOfWork.OrderRepository.GetAllAsync();

            orders = orders.Where(o => o.OrderCode.ToString().ToLower().RemoveDiacritics().Contains(orderCode.ToLower().RemoveDiacritics())).ToList();
            if (orders != null && orders.Any())
            {
                result.StatusCode = 200;
                result.Message = "Order list found by orderCode success";
                result.Data = _mapper.Map<List<OrderResponse>>(orders);
                return result;
            }
            result.StatusCode = 404;
            result.Message = "Order list not found by orderCode: " + orderCode;
            return result;

        }

        #endregion

        #region Get orders by user id
        public async Task<ResponseObject<List<OrderResponse>>> GetOrdersByUserId(Guid userId)
        {
            var result = new ResponseObject<List<OrderResponse>>();
            try
            {
                //tim thong tin cua order lien quan toi id nguoi  dung
                var foundList = _unitOfWork.OrderRepository.Get(x => x.UserId == userId).ToList().OrderByDescending(x => x.OrderDate);
                if (foundList.Count() == 0)
                {
                    result.StatusCode = 500;
                    result.Message = "Don't have order by this user!";
                    return result;
                }

                //tren moblie chi can get trans moi nhat
                //foreach ( var item in foundList )
                //{
                //	if ( item.Transaction != null && item.Transaction.Count > 1 )
                //		item.Transaction = item.Transaction.OrderByDescending(x => x.TransactionDate).Take(1).ToList();
                //}
                result.StatusCode = 200;
                result.Message = "Ok, list order " + foundList.Count();
                var returnList = _mapper.Map<List<OrderResponse>>(foundList);
                result.Data = returnList;
                return result;
            }
            catch (Exception ex)
            {
                result.StatusCode = 500;
                result.Message = ex.Message;
                return result;
            }
        }
        #endregion

        #region get order by id
        public async Task<ResponseObject<OrderResponse?>> GetOrderByIdAsync(Guid id)
        {
            var result = new ResponseObject<OrderResponse?>();
            var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(id.ToString());
            if (orderExist != null)
            {
                result.StatusCode = 200;
                result.Message = "Order:";
                result.Data = _mapper.Map<OrderResponse>(orderExist);
                return result;
            }
            else
            {
                result.StatusCode = 404;
                result.Message = "Order not found!";
                return result;
            }
        }

        #endregion

        #region create
        public async Task<ResponseObject<OrderResponse>> CreateOrderAsync(CreateOrderRequest model)
        {
            var result = new ResponseObject<OrderResponse>();
            var validationResult = _createOrderValidator.Validate(model);
            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                result.StatusCode = 400;
                result.Message = string.Join(" - ", error);
                return result;
            }
            //check userId exists
            var userExist = await _unitOfWork.UserRepository.GetByIdAsync(model.UserId);
            if (userExist == null)
            {
                result.StatusCode = 404;
                result.Message = "User not found!";
                return result;
            }
            //check recipeList
            if (model.RecipeList == null)
            {
                result.StatusCode = 404;
                result.Message = "Recipe list null! Please input recipe!!";
                return result;
            }
            //check wp có được bán hay không
            var wpAvailable = await _unitOfWork.WeeklyPlanRepository.GetByIdAsync(model.StanderdWeeklyPlanId.ToString());
            if (wpAvailable != null)
            {
                if (wpAvailable.BaseStatus == BaseStatus.UnAvailable)
                {
                    result.StatusCode = 400;
                    result.Message = "Kế hoạch tuần (" + wpAvailable.Title + ") hiện không khả dụng. Hệ thống đang trong quá trình cập nhật thông tin. Mong quý khách thông cảm!";
                    return result;
                }
            }

            int quantity = 0;
            foreach (var item in model.RecipeList)//tính số lượng phần ăn (dưới 5 hoặc trên 200 ko cho đặt)
            {
                quantity += item.Quantity;
                var recipe = _unitOfWork.RecipeRepository.Get(r => r.Id.Equals(item.RecipeId.ToString())).FirstOrDefault();
                if (recipe != null && recipe.BaseStatus != BaseStatus.Available && recipe.ProcessStatus != ProcessStatus.Approved)
                {
                    result.StatusCode = 400;
                    result.Message = "Đơn hàng có món ăn (" + recipe.Name + ") không được bán hãy xem lại!";
                    return result;
                }
            }
            if (quantity < 4 || quantity > 200)//ko đáp ứng quy định phần ăn quy định (5-200)
            {
                result.StatusCode = 500;
                result.Message = "Vui lòng đặt ít nhất 4 phần ăn. Nhiều nhất 200 phần";
                return result;
            }
            // Tạo mã đơn hàng duy nhất
            var randomOrderCode = await GenerateUniqueOrderCodeAsync();
            var newOrder = _mapper.Map<Order>(model);
            newOrder.StanderdWeeklyPlanId = null;
            newOrder.OrderImg = wpAvailable?.UrlImage ?? "";
            newOrder.OrderTitle = wpAvailable?.Title ?? "";
            newOrder.OrderCode = randomOrderCode;
            newOrder.OrderDate = DateTime.UtcNow.AddHours(7);
            switch (DateTime.UtcNow.AddHours(7).DayOfWeek)
            {
                case DayOfWeek.Monday:
                    newOrder.ShipDate = DateTime.UtcNow.AddHours(7).AddDays(6);
                    break;
                case DayOfWeek.Tuesday:
                    newOrder.ShipDate = DateTime.UtcNow.AddHours(7).AddDays(5);
                    break;
                case DayOfWeek.Wednesday:
                    newOrder.ShipDate = DateTime.UtcNow.AddHours(7).AddDays(4);
                    break;
                case DayOfWeek.Thursday:
                    newOrder.ShipDate = DateTime.UtcNow.AddHours(7).AddDays(3);
                    break;
                case DayOfWeek.Friday:
                    newOrder.ShipDate = DateTime.UtcNow.AddHours(7).AddDays(2);
                    break;
                case DayOfWeek.Saturday:
                    newOrder.ShipDate = DateTime.UtcNow.AddHours(7).AddDays(8);
                    break;
                case DayOfWeek.Sunday:
                    newOrder.ShipDate = DateTime.UtcNow.AddHours(7).AddDays(7);
                    break;
            }
            newOrder.Status = OrderStatus.Processing;
            var createResult = await _unitOfWork.OrderRepository.CreateAsync(newOrder);
            if (createResult)//bat dau add cac recipeId thanh cac OrderDetail thong qua RecipeList
            {
                await _unitOfWork.CompleteAsync();
                if (model.RecipeList.Any())
                {
                    var createOrderDetailResult = await _orderDetailService.CreateOrderDetailAsync(newOrder.Id, model.RecipeList);
                    if (createOrderDetailResult.StatusCode == 200 && createOrderDetailResult.Data != null)
                    {
                        await _unitOfWork.CompleteAsync();
                        //create transaction
                        //phan biet loai transaction
                        var newPaymentRequest = new CreatePaymentRequest();
                        newPaymentRequest.OrderId = newOrder.Id;
                        newPaymentRequest.Amount = newOrder.TotalPrice;
                        newPaymentRequest.TransactionType = model.TransactionType;
                        var createTransactionResult = await _transactionService.CreateNewPaymentAsync(newPaymentRequest);
                        if (createTransactionResult != null && createTransactionResult.StatusCode == 200 && createTransactionResult.Data != null)
                        {
                            //newOrder.Transaction = _mapper.Map<Transaction>(createTransactionResult.Data);
                            await _unitOfWork.CompleteAsync();
                            result.StatusCode = 200;
                            result.Message = "Create order success";
                            result.Data = _mapper.Map<OrderResponse>(newOrder);
                            return result;
                        }
                        if (createTransactionResult != null && createTransactionResult.StatusCode != 200)//code cu la (createTransaction != null)
                        {
                            // Delete the order if transaction creation fails
                            await _unitOfWork.OrderRepository.DeleteAsync(newOrder.Id.ToString());
                            await _unitOfWork.CompleteAsync();
                            result.StatusCode = createTransactionResult.StatusCode;
                            result.Message = createTransactionResult.Message;
                            return result;
                        }
                    }
                    //luc nay la vi li do gi do ko tao duoc thong tin detail (customPlan cho order) -> xoa order. thong bao ko tao thanh cong
                    await _unitOfWork.OrderRepository.DeleteAsync(newOrder.Id.ToString());//xoa thong tin cho Order
                    await _unitOfWork.CompleteAsync();
                    result.StatusCode = 500;
                    result.Message = "Create order not success!";
                    return result;
                }
            }
            await _unitOfWork.OrderRepository.DeleteAsync(newOrder.Id.ToString());
            await _unitOfWork.CompleteAsync();
            result.StatusCode = 500;
            result.Message = "Create order not success!";
            return result;
        }
        // Hàm để tạo mã đơn hàng ngẫu nhiên và đảm bảo tính duy nhất
        private async Task<int> GenerateUniqueOrderCodeAsync()
        {
            Random random = new Random();
            int minValue = 10000000;
            int maxValue = 99999999;
            int randomOrderCode;
            bool isUnique;

            do
            {
                randomOrderCode = random.Next(minValue, maxValue + 1);
                // Kiểm tra xem mã đơn hàng đã tồn tại trong cơ sở dữ liệu chưa
                isUnique = !await _unitOfWork.OrderRepository.Get(x => x.OrderCode == randomOrderCode).AnyAsync();
            }
            while (!isUnique); // Lặp lại cho đến khi tìm thấy mã đơn hàng duy nhất

            return randomOrderCode;
        }
        #endregion

        #region update order
        public async Task<ResponseObject<OrderResponse>> UpdateOrderAsync(string id, UpdateOrderRequest model) //ham nay ko dung den - xoa?
        {
            var result = new ResponseObject<OrderResponse>();
            var validationResult = _updateOrderValidator.Validate(model);
            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                result.StatusCode = 400;
                result.Message = string.Join(" - ", error);
                return result;
            }
            //find order
            var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(id.ToString());
            if (orderExist != null)
            {
                var updateOrder = _mapper.Map<Order>(model);
                updateOrder.Id = orderExist.Id;
                var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(updateOrder);
                if (updateResult)
                {
                    await _unitOfWork.CompleteAsync();
                    result.StatusCode = 200;
                    result.Message = "Update successfully.";
                    result.Data = _mapper.Map<OrderResponse>(updateOrder);
                    return result;
                }
                else
                {
                    result.StatusCode = 500;
                    result.Message = "Update faild!";
                    return result;
                }
            }
            else
            {
                result.StatusCode = 404;
                result.Message = "Order not found!";
                return result;
            }
        }
        public async Task<ResponseObject<OrderResponse>> ChangeOrderGroupAsync(Guid idOrder, Guid idOrderGroup)
        {
            var result = new ResponseObject<OrderResponse>();

            //check order group exist
            var orderGroupExist = await _unitOfWork.OrderGroupRepository.GetByIdAsync(idOrderGroup.ToString());

            if (orderGroupExist != null)
            {
                var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(idOrder.ToString());
                if (orderExist != null)
                {
                    //nếu order đang được shipper giao thì không được đổi order group khác 
                    if (orderExist.Status == OrderStatus.Shipping)
                    {
                        result.StatusCode = 400;
                        result.Message = "Orders being delivered cannot be changed!";
                        return result;
                    }
                    orderExist.OrderGroupId = idOrderGroup;
                    orderExist.OrderGroup = orderGroupExist;
                    await _unitOfWork.CompleteAsync();
                    result.StatusCode = 200;
                    result.Message = "Change order group success";
                    return result;
                }
            }

            result.StatusCode = 404;
            result.Message = "OrderGroup not exist!";
            return result;
        }

        #endregion

        #region update order by user ...?
        public async Task<ResponseObject<OrderResponse>> UpdateOrderByUserAsync(UpdateOrderByUserRequest model)
        {
            var result = new ResponseObject<OrderResponse>();
            var validationResult = _updateOrderByUserValidator.Validate(model);
            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                result.StatusCode = 400;
                result.Message = string.Join(" - ", error);
                return result;
            }
            //find order
            var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(model.Id.ToString());
            if (orderExist != null)
            {
                var updateOrder = _mapper.Map<Order>(model);
                var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(updateOrder);
                if (updateResult)
                {
                    await _unitOfWork.CompleteAsync();
                    result.StatusCode = 200;
                    result.Message = "Update successfully.";
                    result.Data = _mapper.Map<OrderResponse>(updateOrder);
                    return result;
                }
                else
                {
                    result.StatusCode = 500;
                    result.Message = "Update faild!";
                    return result;
                }
            }
            else
            {
                result.StatusCode = 404;
                result.Message = "Order not found!";
                return result;
            }
        }

        #endregion

        #region delete
        public async Task<ResponseObject<OrderResponse>> DeleteOrderAsync(Guid id)
        {
            var result = new ResponseObject<OrderResponse>();

            var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(id.ToString());
            if (orderExist != null)
            {
                var userExist = await _unitOfWork.OrderRepository.GetUserExistInOrderAsync(orderExist.Id, orderExist.UserId);
                if (userExist)
                {
                    //change order status
                    orderExist.Status = OrderStatus.Canceled;
                    var orderUpdate = await _unitOfWork.OrderRepository.UpdateAsync(orderExist);
                    if (orderUpdate)
                    {
                        await _unitOfWork.CompleteAsync();
                        result.StatusCode = 200;
                        result.Message = "Change status order (" + orderExist.Id + ") into canceled success.";
                        return result;
                    }
                    else
                    {
                        result.StatusCode = 500;
                        result.Message = "Fail to change status went delete order!";
                        return result;
                    }
                }
                else
                {
                    var orderDelete = await _unitOfWork.OrderRepository.DeleteAsync(orderExist.Id.ToString());
                    if (orderDelete)
                    {
                        await _unitOfWork.CompleteAsync();
                        result.StatusCode = 200;
                        result.Message = "Delete order success.";
                        result.Data = _mapper.Map<OrderResponse>(orderExist);
                        return result;
                    }
                    else
                    {
                        result.StatusCode = 500;
                        result.Message = "Delete order unsucces!";
                        return result;
                    }
                }
            }
            result.StatusCode = 404;
            result.Message = "Order not exist!";
            return result;
        }
        public async Task<ResponseObject<OrderResponse>> RemoveOrderFormOrderGroupAsync(Guid idOrder)
        {
            var result = new ResponseObject<OrderResponse>();

            var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(idOrder.ToString());
            if (orderExist != null)
            {
                orderExist.OrderGroupId = null;
                orderExist.OrderGroup = null;
                var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(orderExist);
                if (updateResult)
                {
                    await _unitOfWork.CompleteAsync();
                    result.StatusCode = 200;
                    result.Message = "Remove Order from orde rgoup success";
                    return result;
                }
                result.StatusCode = 500;
                result.Message = "Remove Order from orde rgoup unsuccess!";
                return result;
            }
            result.StatusCode = 404;
            result.Message = "Order not exist!";
            return result;

        }

        public async Task<ResponseObject<List<OrderResponse>>> RemoveAllOrdersFromOrderGroupsAsync()
        {
            var result = new ResponseObject<List<OrderResponse>>();
            var orderResponses = new List<OrderResponse>();

            try
            {
                // Lấy tất cả các order có OrderGroupId không null
                var ordersWithOrderGroups = await _unitOfWork.OrderRepository.GetAllAsync();
                ordersWithOrderGroups = ordersWithOrderGroups.Where(o => o.OrderGroupId != null).ToList();

                if (ordersWithOrderGroups.Any())
                {
                    foreach (var order in ordersWithOrderGroups)
                    {
                        order.OrderGroupId = null;
                        order.OrderGroup = null;
                        var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(order);

                        if (updateResult)
                        {
                            orderResponses.Add(_mapper.Map<OrderResponse>(order));
                        }
                        else
                        {
                            result.StatusCode = 500;
                            result.Message = "An error occurred while updating orders.";
                            return result;
                        }
                    }

                    await _unitOfWork.CompleteAsync();
                    result.StatusCode = 200;
                    result.Message = "Successfully removed all orders from their order groups.";
                    result.Data = orderResponses;
                    return result;
                }
                else
                {
                    result.StatusCode = 404;
                    result.Message = "No orders found with an order group.";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.StatusCode = 500;
                result.Message = $"Error when processing: {ex.Message}";
                return result;
            }
        }
        #endregion

        #region Change status
        public async Task<ResponseObject<OrderResponse>> TestChangeStatus(Guid id, ChangeStatusOrderRequest model)
        {
            var result = new ResponseObject<OrderResponse>();
            var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(id.ToString());
            //bat dau
            if (!orderExist.Id.ToString().IsNullOrEmpty())
            {
                //Confirm
                if (model.Status == OrderStatus.Confirm)//Confirm khi cluster va Confirm thu cong
                {//check transaction COD va Zalopay-PAID thì cho confirm
                    if ((orderExist.Status == OrderStatus.Processing && orderExist.Transaction.Type == TransactionType.COD) //dang o Processing va transaction type la COD thi cho confirm
                        || (orderExist.Status == OrderStatus.Processing && orderExist.Transaction.Type == TransactionType.ZaloPay && orderExist.Transaction.Status == TransactionStatus.PAID) //transaction type la zalopay va status cua transaction la PAID thi cho confirm
                        || orderExist.Status == OrderStatus.UnShipped)//status cua order la Unshipped thi cho confirm
                    {//sua them cho Unshipped cong them dieu kien la COD hoac la Zalopay da PAID
                        orderExist.Status = OrderStatus.Confirm;
                        await _unitOfWork.CompleteAsync();
                        result.StatusCode = 200;
                        result.Message = "Change order status into " + orderExist.Status + " success.";
                        result.Data = _mapper.Map<OrderResponse>(orderExist);
                        return result;
                    }
                    else if (orderExist.Status == OrderStatus.Processing && orderExist.Transaction.Type == TransactionType.ZaloPay && orderExist.Transaction.Status == TransactionStatus.Pending)//truong hop co Zalopay nhung chua thanh toan
                    {
                        result.StatusCode = 200;
                        result.Message = "1.Order Zalopay not paid yet";
                        return result;
                    }
                    else //cac truong hop khac
                    {
                        result.StatusCode = 500;
                        result.Message = "Error when change status to " + model.Status;
                        return result;
                    }
                }

                //Shipping
                if (model.Status == OrderStatus.Shipping)//Shipper nhan shipping don hang tren app mobile khi 
                {
                    if (orderExist.Status == OrderStatus.Confirm) //chi khi confirm moi cho shipping
                    {
                        orderExist.Status = OrderStatus.Shipping;
                        await _unitOfWork.CompleteAsync();
                        result.StatusCode = 200;
                        result.Message = "Change order status into " + orderExist.Status + " success.";
                        return result;
                    }
                    else
                    {
                        result.StatusCode = 500;
                        result.Message = "Order status need to be Confirm before change into " + model.Status;
                        return result;
                    }
                }

                //Unshipped
                if (model.Status == OrderStatus.UnShipped && !model.Message.IsNullOrEmpty())//shipper thong bao giao hang khong thanh cong, shipper can nhap them note
                {
                    if (orderExist.Status == OrderStatus.Shipping)
                    {
                        orderExist.Status = OrderStatus.UnShipped;
                        orderExist.Message = orderExist.Message + "_" + model.Message;
                        if (!model.Img.IsNullOrEmpty())//truong hop can chup lai
                        {
                            orderExist.Img = model.Img;
                        }
                        var updateStatusResult = await _unitOfWork.OrderRepository.UpdateAsync(orderExist);
                        if (updateStatusResult)
                        {
                            await _unitOfWork.CompleteAsync();
                            result.StatusCode = 200;
                            result.Message = "Change order status into " + orderExist.Status + " success.";
                            return result;
                        }
                        else
                        {
                            result.StatusCode = 500;
                            result.Message = "Change order status into " + model.Status + " not success.";
                            return result;
                        }
                    }
                    else
                    {
                        result.StatusCode = 500;
                        result.Message = "Error when change status to " + model.Status;
                        return result;
                    }
                }
                else if (model.Status == OrderStatus.UnShipped && model.Message.IsNullOrEmpty())//ko co note
                {
                    result.StatusCode = 500;
                    result.Message = "Unshipped status update must come with note from shipper ";
                    return result;
                }

                //Shipped
                if (model.Status == OrderStatus.Shipped && !model.Img.IsNullOrEmpty())//shipper da giao hang thanh cong
                {
                    orderExist.Status = OrderStatus.Shipped;
                    orderExist.Img = model.Img;
                    orderExist.Transaction.Status = TransactionStatus.PAID;//chuyen status cua transaction lien quan thanh PAID
                    if (!model.Message.IsNullOrEmpty())//truong hop can note them
                    {
                        orderExist.Message += "_" + model.Message;
                    }
                    foreach (var orderDetail in orderExist.OrderDetails)//tăng popularity của các recipe tương ứng
                    {
                        var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(orderDetail.RecipeId.ToString());
                        if (recipeExist != null)
                        {
                            recipeExist.Popularity++;
                        }
                    }
                    var updateStatusResult = await _unitOfWork.OrderRepository.UpdateAsync(orderExist);
                    if (updateStatusResult)
                    {

                        await _unitOfWork.CompleteAsync();
                        result.StatusCode = 200;
                        result.Message = "Change order status into " + orderExist.Status + " success.";
                        return result;
                    }
                    else
                    {
                        result.StatusCode = 500;
                        result.Message = "Change order status into " + model.Status + " not success.";
                        return result;
                    }
                }
                else if (model.Status == OrderStatus.Shipped && model.Img.IsNullOrEmpty())//ko co img
                {
                    result.StatusCode = 500;
                    result.Message = "Shipped status update must come with confirm image from shipper ";
                    return result;
                }

                //Delivered
                if (model.Status == OrderStatus.Delivered && orderExist.Status == OrderStatus.Shipped)//khach hang chon confirm da nhan duoc hang roi
                {
                    orderExist.Status = OrderStatus.Delivered;
                    await _unitOfWork.CompleteAsync();
                    result.StatusCode = 200;
                    result.Message = "Change order status into " + orderExist.Status + " success.";
                    return result;
                }
                else if (model.Status == OrderStatus.Delivered && orderExist.Status != OrderStatus.Shipped)
                {
                    result.StatusCode = 500;
                    result.Message = "Delivered status update must come after with Shipped confirm from shipper ";
                    return result;
                }

                //Cancel
                if (model.Status == OrderStatus.Canceled)
                {
                    if (orderExist.Status == OrderStatus.Processing || orderExist.Status == OrderStatus.UnShipped || orderExist.Status == OrderStatus.Shipped)//ko giao đc hang hoac khach dong don
                    {// + transaction COD hoac Zalopay Pending - khach ko muon giao lai lan nua hoac la chua thanh toan cho don hang
                        if (orderExist.Transaction.Status == TransactionStatus.Pending)//don hang chua thanh toan
                        {
                            orderExist.Status = OrderStatus.Canceled;
                            orderExist.Transaction.Status = TransactionStatus.UNPAID;
                            await _unitOfWork.CompleteAsync();
                            result.StatusCode = 200;
                            result.Message = "Change order status into " + orderExist.Status + " success.";
                            result.Data = _mapper.Map<OrderResponse>(orderExist);
                            return result;
                        }
                        else if (orderExist.Transaction.Status == TransactionStatus.PAID)//don hang da duoc thanh toan -> khach hang can duoc lien lac va refund
                        {
                            orderExist.Status = OrderStatus.Canceled;
                            orderExist.Transaction.Status = TransactionStatus.RefundPending;
                            await _unitOfWork.CompleteAsync();
                            result.StatusCode = 200;
                            result.Message = "This order need to be refunded. Change order status into " + orderExist.Status + " success.";
                            result.Data = _mapper.Map<OrderResponse>(orderExist);
                            return result;
                        }
                    }
                    else
                    {
                        result.StatusCode = 500;
                        result.Message = "Not fit with pre-condition.(Order on Processing|Order Unshipped|Order been Shippped but Custopmer want refund)";
                        return result;
                    }
                }

                //nếu order đang ở trạng thái cancel và đã có transaction paid thì mới được đổi sang refund
                if (orderExist.Status == OrderStatus.Canceled && model.Status == OrderStatus.Refund && orderExist.Transaction != null)
                {
                    //check transaction
                    var transactionExist = await _unitOfWork.TransactionRepository.GetByIdAsync(orderExist.Transaction.Id.ToString());
                    if (transactionExist != null && transactionExist.Status != TransactionStatus.PAID)
                    {
                        result.StatusCode = 400;
                        result.Message = "Can't change order status into " + model.Status + " because order not paid.";
                        return result;
                    }
                    transactionExist.Status = TransactionStatus.RefundDone;
                }
                orderExist.Status = model.Status;
                var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(orderExist);
                if (updateResult)
                {
                    await _unitOfWork.CompleteAsync();
                    result.StatusCode = 200;
                    result.Message = "Change order status into " + orderExist.Status + " success.";
                    var mapOrderResponse = _mapper.Map<OrderResponse>(orderExist);
                    var customer = await _unitOfWork.UserRepository.GetByIdAsync(orderExist.UserId.ToString());
                    if (customer != null)
                    {
                        mapOrderResponse.UserId = customer.Email;
                    }
                    result.Data = mapOrderResponse;
                    return result;
                }
                else
                {
                    result.StatusCode = 500;
                    result.Message = "Fail to update order!";
                    return result;
                }
            }
            result.StatusCode = 404;
            result.Message = "Order not exist!";
            return result;

        }
        #endregion

        #region test
        public async Task<ResponseObject<OrderResponse>> ChangeStatusOrderAsync(Guid id, ChangeStatusOrderRequest model)
        {
            /*
             bắt đầu xét trường hợp, bối cảnh để chuyển status của order
            .Processing - ko có điều kiện - status lúc mới tạo

            .Confirm - hình thành sau khi Cluster - check thêm thông tin transaction 
                + Zalopay mà Paid thì chuyển thành Confirm
                + Zalopay mà Pending thì không chuyển thành Confirm

            .Shipping chỉ cho chuyển khi order có status hiện tại là Confirm

            .Unshipped phải đi chung với ShipperNote. Từ Unshipped có thể ngược lại thành Confirm hoặc thành Canceled

            .Shipped được phép chuyển thành khi đi kèm với Img

            .Delivered được phép chuyển khi order đang ở status Shipped

            .Canceled +có thể chuyển khi order đang ở Processing
                    +có thể chuyển từ Unshipped sang
            - điều kiện thêm cho Canceled
                    + khi Canceled phải check transaction là COD hay Zalopay
                nếu là Zalopay thì check 
                    + Pending thì ko sao
                    + Paid thì chuyển status của transaction thành RefundZaloPayPending
             */
            var result = new ResponseObject<OrderResponse>();
            var orderExist = await _unitOfWork.OrderRepository.GetByIdAsync(id.ToString());
            //bat dau
            if (!orderExist.Id.ToString().IsNullOrEmpty())
            {
                //Confirm
                if (model.Status == OrderStatus.Confirm)//Confirm khi cluster va Confirm thu cong
                {//check transaction COD va Zalopay-PAID thì cho confirm
                    if ((orderExist.Status == OrderStatus.Processing && orderExist.Transaction.Type == TransactionType.COD) //dang o Processing va transaction type la COD thi cho confirm
                        || (orderExist.Status == OrderStatus.Processing && orderExist.Transaction.Type == TransactionType.ZaloPay && orderExist.Transaction.Status == TransactionStatus.PAID) //transaction type la zalopay va status cua transaction la PAID thi cho confirm
                        || orderExist.Status == OrderStatus.UnShipped)//status cua order la Unshipped thi cho confirm
                    {//sua them cho Unshipped cong them dieu kien la COD hoac la Zalopay da PAID
                        orderExist.Status = OrderStatus.Confirm;
                        await _unitOfWork.CompleteAsync();
                        result.StatusCode = 200;
                        result.Message = "Change order status into " + orderExist.Status + " success.";
                        result.Data = _mapper.Map<OrderResponse>(orderExist);
                        return result;
                    }
                    else if (orderExist.Status == OrderStatus.Processing && orderExist.Transaction.Type == TransactionType.ZaloPay && orderExist.Transaction.Status == TransactionStatus.Pending)//truong hop co Zalopay nhung chua thanh toan
                    {
                        result.StatusCode = 200;
                        result.Message = "Order Zalopay not paid yet!";
                        var mapOrderResponse = _mapper.Map<OrderResponse>(orderExist);
                        var customer = await _unitOfWork.UserRepository.GetByIdAsync(orderExist.UserId.ToString());
                        if (customer != null)
                        {
                            mapOrderResponse.UserId = customer.Email;
                        }
                        result.Data = mapOrderResponse;
                        return result;
                    }
                    else //cac truong hop khac
                    {
                        result.StatusCode = 500;
                        result.Message = "Error when change status to " + model.Status;
                        return result;
                    }
                }

                //Shipping
                if (model.Status == OrderStatus.Shipping)//Shipper nhan shipping don hang tren app mobile khi 
                {
                    if (orderExist.Status == OrderStatus.Confirm) //chi khi confirm moi cho shipping
                    {
                        orderExist.Status = OrderStatus.Shipping;
                        await _unitOfWork.CompleteAsync();
                        result.StatusCode = 200;
                        result.Message = "Change order status into " + orderExist.Status + " success.";
                        var mapOrderResponse = _mapper.Map<OrderResponse>(orderExist);
                        var customer = await _unitOfWork.UserRepository.GetByIdAsync(orderExist.UserId.ToString());
                        if (customer != null)
                        {
                            mapOrderResponse.UserId = customer.Email;
                        }
                        result.Data = mapOrderResponse;
                        return result;
                    }
                    else
                    {
                        result.StatusCode = 500;
                        result.Message = "Order status need to be Confirm before change into " + model.Status;
                        return result;
                    }
                }

                //Unshipped
                if (model.Status == OrderStatus.UnShipped && !model.Message.IsNullOrEmpty())//shipper thong bao giao hang khong thanh cong, shipper can nhap them note
                {
                    if (orderExist.Status == OrderStatus.Shipping)
                    {
                        orderExist.Status = OrderStatus.UnShipped;
                        orderExist.Message = orderExist.Message + "_" + model.Message;
                        if (!model.Img.IsNullOrEmpty())//truong hop can chup lai
                        {
                            orderExist.Img = model.Img;
                        }
                        var updateStatusResult = await _unitOfWork.OrderRepository.UpdateAsync(orderExist);
                        if (updateStatusResult)
                        {
                            await _unitOfWork.CompleteAsync();
                            result.StatusCode = 200;
                            result.Message = "Change order status into " + orderExist.Status + " success.";
                            var mapOrderResponse = _mapper.Map<OrderResponse>(orderExist);
                            var customer = await _unitOfWork.UserRepository.GetByIdAsync(orderExist.UserId.ToString());
                            if (customer != null)
                            {
                                mapOrderResponse.UserId = customer.Email;
                            }
                            result.Data = mapOrderResponse;
                            return result;
                        }
                        else
                        {
                            result.StatusCode = 500;
                            result.Message = "Change order status into " + model.Status + " not success.";
                            return result;
                        }
                    }
                    else
                    {
                        result.StatusCode = 500;
                        result.Message = "Error when change status to " + model.Status;
                        return result;
                    }
                }
                else if (model.Status == OrderStatus.UnShipped && model.Message.IsNullOrEmpty())//ko co note
                {
                    result.StatusCode = 500;
                    result.Message = "Unshipped status update must come with note from shipper ";
                    return result;
                }

                //Shipped
                if (model.Status == OrderStatus.Shipped && !model.Img.IsNullOrEmpty())//shipper da giao hang thanh cong
                {
                    orderExist.Status = OrderStatus.Shipped;
                    orderExist.Img = model.Img;
                    orderExist.Transaction.Status = TransactionStatus.PAID;//chuyen status cua transaction lien quan thanh PAID
                    if (!model.Message.IsNullOrEmpty())//truong hop can note them
                    {
                        orderExist.Message += "_" + model.Message;
                    }
                    foreach (var orderDetail in orderExist.OrderDetails)//tăng popularity của các recipe tương ứng
                    {
                        var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(orderDetail.RecipeId.ToString());
                        if (recipeExist != null)
                        {
                            recipeExist.Popularity++;
                        }
                    }
                    var updateStatusResult = await _unitOfWork.OrderRepository.UpdateAsync(orderExist);
                    if (updateStatusResult)
                    {

                        await _unitOfWork.CompleteAsync();
                        result.StatusCode = 200;
                        result.Message = "Change order status into " + orderExist.Status + " success.";
                        var mapOrderResponse = _mapper.Map<OrderResponse>(orderExist);
                        var customer = await _unitOfWork.UserRepository.GetByIdAsync(orderExist.UserId.ToString());
                        if (customer != null)
                        {
                            mapOrderResponse.UserId = customer.Email;
                        }
                        result.Data = mapOrderResponse;
                        return result;
                    }
                    else
                    {
                        result.StatusCode = 500;
                        result.Message = "Change order status into " + model.Status + " not success.";
                        return result;
                    }
                }
                else if (model.Status == OrderStatus.Shipped && model.Img.IsNullOrEmpty())//ko co img
                {
                    result.StatusCode = 500;
                    result.Message = "Shipped status update must come with confirm image from shipper ";
                    return result;
                }

                //Delivered
                if (model.Status == OrderStatus.Delivered && orderExist.Status == OrderStatus.Shipped)//khach hang chon confirm da nhan duoc hang roi
                {
                    orderExist.Status = OrderStatus.Delivered;
                    await _unitOfWork.CompleteAsync();
                    result.StatusCode = 200;
                    result.Message = "Change order status into " + orderExist.Status + " success.";
                    var mapOrderResponse = _mapper.Map<OrderResponse>(orderExist);
                    var customer = await _unitOfWork.UserRepository.GetByIdAsync(orderExist.UserId.ToString());
                    if (customer != null)
                    {
                        mapOrderResponse.UserId = customer.Email;
                    }
                    result.Data = mapOrderResponse;
                    return result;
                }
                else if (model.Status == OrderStatus.Delivered && orderExist.Status != OrderStatus.Shipped)
                {
                    result.StatusCode = 500;
                    result.Message = "Delivered status update must come after with Shipped confirm from shipper ";
                    return result;
                }

                //Cancel
                if (model.Status == OrderStatus.Canceled)
                {
                    if (orderExist.Status == OrderStatus.Processing || orderExist.Status == OrderStatus.UnShipped || orderExist.Status == OrderStatus.Shipped)//ko giao đc hang hoac khach dong don
                    {// + transaction COD hoac Zalopay Pending - khach ko muon giao lai lan nua hoac la chua thanh toan cho don hang
                        if (orderExist.Transaction != null && orderExist.Transaction.Status == TransactionStatus.Pending)//don hang chua thanh toan
                        {
                            orderExist.Status = OrderStatus.Canceled;
                            orderExist.Transaction.Status = TransactionStatus.UNPAID;
                            await _unitOfWork.CompleteAsync();
                            result.StatusCode = 200;
                            result.Message = "Change order status into " + orderExist.Status + " success.";
                            var mapOrderResponse = _mapper.Map<OrderResponse>(orderExist);
                            var customer = await _unitOfWork.UserRepository.GetByIdAsync(orderExist.UserId.ToString());
                            if (customer != null)
                            {
                                mapOrderResponse.UserId = customer.Email;
                            }
                            result.Data = mapOrderResponse;
                            return result;
                        }
                        else if (orderExist.Transaction.Status == TransactionStatus.PAID)//don hang da duoc thanh toan -> khach hang can duoc lien lac va refund
                        {
                            orderExist.Status = OrderStatus.Canceled;
                            orderExist.Transaction.Status = TransactionStatus.RefundPending;
                            await _unitOfWork.CompleteAsync();
                            result.StatusCode = 200;
                            result.Message = "This order need to be refunded. Change order status into " + orderExist.Status + " success.";
                            var mapOrderResponse = _mapper.Map<OrderResponse>(orderExist);
                            var customer = await _unitOfWork.UserRepository.GetByIdAsync(orderExist.UserId.ToString());
                            if (customer != null)
                            {
                                mapOrderResponse.UserId = customer.Email;
                            }
                            result.Data = mapOrderResponse;
                            return result;
                        }
                    }
                    else
                    {
                        result.StatusCode = 500;
                        result.Message = "Not fit with pre-condition.(Order on Processing|Order Unshipped|Order been Shippped but Custopmer want refund)";
                        return result;
                    }
                }

                if (model.Status == OrderStatus.Refund)//refund
                {
                    //nếu order đang ở trạng thái cancel và đã có transaction paid thì mới được đổi sang refund
                    if (orderExist.Status == OrderStatus.Canceled && model.Status == OrderStatus.Refund && orderExist.Transaction != null)
                    {
                        //check transaction
                        var transactionExist = await _unitOfWork.TransactionRepository.GetByIdAsync(orderExist.Transaction.Id.ToString());
                        if (transactionExist != null && transactionExist.Status != TransactionStatus.PAID)
                        {
                            result.StatusCode = 400;
                            result.Message = "Can't change order status into " + model.Status + " because order not paid.";
                            return result;
                        }
                        transactionExist.Status = TransactionStatus.RefundDone;
                    }
                    orderExist.Status = model.Status;
                    var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(orderExist);
                    if (updateResult)
                    {
                        await _unitOfWork.CompleteAsync();
                        result.StatusCode = 200;
                        result.Message = "Change order status into " + orderExist.Status + " success.";
                        var mapOrderResponse = _mapper.Map<OrderResponse>(orderExist);
                        var customer = await _unitOfWork.UserRepository.GetByIdAsync(orderExist.UserId.ToString());
                        if (customer != null)
                        {
                            mapOrderResponse.UserId = customer.Email;
                        }
                        result.Data = mapOrderResponse;
                        return result;
                    }
                    else
                    {
                        result.StatusCode = 500;
                        result.Message = "Fail to update order!";
                        return result;
                    }
                }

            }
            result.StatusCode = 404;
            result.Message = "Order not exist!";
            return result;
        }
        #endregion
    }
}
