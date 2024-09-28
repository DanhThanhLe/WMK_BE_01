using Accord.MachineLearning;
using Accord.Math.Distances;
using AutoMapper;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderGroupModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderGroupModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;
using static Accord.MachineLearning.KMeansClusterCollection;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class OrderGroupService : IOrderGroupService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		#region Validator
		private readonly CreateOrderGroupModelValidator _createValidator;
		private readonly UpdateOrderGroupModelValidator _updateValidator;
		#endregion


		public OrderGroupService(IUnitOfWork unitOfWork , IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_createValidator = new CreateOrderGroupModelValidator();
			_updateValidator = new UpdateOrderGroupModelValidator();
		}

		#region Get
		public async Task<ResponseObject<List<OrderGroupsResponse>>> GetAllAsync(GetALLOrderGroupsRequest? model)
		{
			var result = new ResponseObject<List<OrderGroupsResponse>>();
			var orderGroups = new List<OrderGroup>();
			var orderGroupsResponse = new List<OrderGroupsResponse>();
			if ( model != null && !model.Location.IsNullOrEmpty() )
			{
				if ( !model.Location.IsNullOrEmpty() )
				{
					var orderGroupsByLocation = await GetOrderGroupsByLocation(model.Location);
					if ( orderGroupsByLocation != null && orderGroupsByLocation.Data != null )
					{
						orderGroupsResponse.AddRange(orderGroupsByLocation.Data);
					}
					// Loại bỏ các phần tử trùng lặp dựa trên Id
					orderGroupsResponse = orderGroupsResponse
						.GroupBy(c => c.Id)
						.Select(g => g.First())
						.ToList();
				}
			}
			else
			{
				orderGroups = await _unitOfWork.OrderGroupRepository.GetAllAsync();
				orderGroupsResponse = _mapper.Map<List<OrderGroupsResponse>>(orderGroups);
			}
			if ( orderGroupsResponse != null && orderGroupsResponse.Any() )
			{

				foreach ( var odg in orderGroupsResponse )
				{
					var userExist = await _unitOfWork.UserRepository.GetByIdAsync(odg.ShipperId.ToString());
					if ( userExist != null )
					{
						odg.ShipperUserName = userExist.UserName;
					}
					var staffExist = await _unitOfWork.UserRepository.GetByIdAsync(odg.AsignBy.ToString());
					if ( staffExist != null )
					{
						odg.AsignBy = staffExist.UserName;
					}

				}
				result.StatusCode = 200;
				result.Message = "OrderGroup list";
				result.Data = orderGroupsResponse.OrderBy(og => og.Location).ToList();
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Dont have order group!";
			result.Data = orderGroupsResponse ?? [];
			return result;
		}
		public async Task<ResponseObject<List<OrderGroupsResponse>>> GetOrderGroupsByLocation(string location)
		{
			var result = new ResponseObject<List<OrderGroupsResponse>>();
			var orderGroups = await _unitOfWork.OrderGroupRepository.GetAllAsync();
			orderGroups = orderGroups.Where(od => od.Location.ToLower().RemoveDiacritics().Contains(location.ToLower().RemoveDiacritics())).ToList();
			if ( orderGroups != null && orderGroups.Any() )
			{
				result.StatusCode = 200;
				result.Message = "Order group list by location success";
				result.Data = _mapper.Map<List<OrderGroupsResponse>>(orderGroups);
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Dont have order group!";
			return result;
		}
		public async Task<ResponseObject<OrderGroupsResponse?>> GetOrderGroupByIdAsync(Guid orderGroupId)
		{
			var result = new ResponseObject<OrderGroupsResponse?>();
			var orderGroupExist = await _unitOfWork.OrderGroupRepository.GetByIdAsync(orderGroupId.ToString());
			if ( orderGroupExist != null )
			{

				string userName = null;
				if ( orderGroupExist.AsignBy.ToString() != null )
				{
					userName = _unitOfWork.UserRepository.GetUserNameById(orderGroupExist.AsignBy);
				}
				var returnData = _mapper.Map<OrderGroupsResponse>(orderGroupExist);
				if ( userName != null )
				{
					returnData.AsignBy = userName;
				}

				result.StatusCode = 200;
				result.Message = "OrderGroup";
				result.Data = returnData;
				return result;
			}
			else
			{
				result.StatusCode = 200;
				result.Message = "OrderGroup not exist!";
				return result;
			}
		}
		#endregion

		public async Task<ResponseObject<OrderGroupsResponse>> CreateOrderGroupAsync(CreateOrderGroupRequest model , string assignedBy)
		{
			var result = new ResponseObject<OrderGroupsResponse>();
			var validationResult = _createValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			//check shipper exist
			var shipperExist = await _unitOfWork.UserRepository.GetByIdAsync(model.ShipperId.ToString());
			if ( shipperExist != null && shipperExist.Role != Role.Shipper )
			{
				result.StatusCode = 403;
				result.Message = "Không phải shipper";
				return result;
			}
			else if ( shipperExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Shipper Không tồn tại";
				return result;
			}
			//check orderGroup exist shipper
			var existOrderGroup = _unitOfWork.OrderGroupRepository.GetAll().FirstOrDefault(od => od.ShipperId == shipperExist.Id);
			if ( existOrderGroup != null )
			{
				result.StatusCode = 409;
				result.Message = "Shipper này đang phụ trách Order group khác";
				return result;
			}
			var staffExist = await _unitOfWork.UserRepository.GetByIdAsync(assignedBy.ToString());
			if ( staffExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Người dùng không tồn tại!";
				return result;
			}
			var orderGroupModel = _mapper.Map<OrderGroup>(model);
			Guid idConvert;
			Guid.TryParse(assignedBy , out idConvert);
			orderGroupModel.AsignBy = idConvert;
			orderGroupModel.AsignAt = DateTime.UtcNow.AddHours(7);
			orderGroupModel.Status = BaseStatus.Available;
			orderGroupModel.User = shipperExist;
			var createResult = await _unitOfWork.OrderGroupRepository.CreateAsync(orderGroupModel);
			if ( createResult )
			{
				await _unitOfWork.CompleteAsync();
				shipperExist.OrderGroup = orderGroupModel;
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Tạo thành công";
				result.Data = _mapper.Map<OrderGroupsResponse>(orderGroupModel);
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Tạo không thành công";
				return result;
			}
		}

		public async Task<ResponseObject<OrderGroupsResponse>> UpdateOrderGroupAsync(UpdateOrderGroupRequest model , string id)
		{
			var result = new ResponseObject<OrderGroupsResponse>();
			var validationResult = _updateValidator.Validate(model);
			if ( !validationResult.IsValid )
			{
				var error = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}

			//check new shipper
			var shipperExist = await _unitOfWork.UserRepository.GetByIdAsync(model.ShipperId.ToString());
			if ( shipperExist != null && shipperExist.Role != Role.Shipper )
			{
				result.StatusCode = 403;
				result.Message = "Không phải shipper";
				return result;
			}
			else if ( shipperExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Shipper không tồn tại";
				return result;
			}
			var orderGroupExist = await _unitOfWork.OrderGroupRepository.GetByIdAsync(id.ToString());
			if ( orderGroupExist != null )
			{

				//xóa đi shipper cũ 
				var oldShipper = await _unitOfWork.UserRepository.GetByIdAsync(orderGroupExist.ShipperId.ToString());
				if ( oldShipper != null )
				{
					oldShipper.OrderGroup = null;
				}
				var orderGroup = _mapper.Map(model , orderGroupExist);
				var updateResult = await _unitOfWork.OrderGroupRepository.UpdateAsync(orderGroupExist);
				if ( updateResult )
				{
					orderGroupExist.Status = BaseStatus.Available;
					orderGroupExist.User = shipperExist;
					shipperExist.OrderGroup = orderGroupExist;
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Cập nhật thành công";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Cập nhật không thành công";
					return result;
				}
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Order group không tồn tại";
				return result;
			}

		}

		public async Task<ResponseObject<OrderGroupsResponse>> DeleteOrderGroupAsync(Guid id)
		{
			var result = new ResponseObject<OrderGroupsResponse>();
			var orderGroupExist = await _unitOfWork.OrderGroupRepository.GetByIdAsync(id.ToString());
			if ( orderGroupExist != null )
			{
				var orderGroupExistOrder = _unitOfWork.OrderGroupRepository.OrderGroupExistOrder(orderGroupExist);
				if ( orderGroupExistOrder )
				{
					//change status
					orderGroupExist.Status = BaseStatus.UnAvailable;
					var updateResult = await _unitOfWork.OrderGroupRepository.UpdateAsync(orderGroupExist);
					if ( updateResult )
					{
						await _unitOfWork.CompleteAsync();
						result.StatusCode = 200;
						result.Message = "Chuyển trạng thái thành công - order group đang chứa đơn hàng";
						return result;
					}
					else
					{
						result.StatusCode = 500;
						result.Message = "Đổi trạng thái không thành công";
						return result;
					}
				}
				else
				{
					var deleteResult = await _unitOfWork.OrderGroupRepository.DeleteAsync(id.ToString());
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
			else
			{
				result.StatusCode = 404;
				result.Message = "OrderGroup không tồn tại";
				return result;
			}
		}

		public async Task<ResponseObject<OrderGroupsResponse>> ChangeStatusOrderGroupAsync(Guid id , ChangeStatusOrderGroupRequest model)
		{
			var result = new ResponseObject<OrderGroupsResponse>();
			var orderGroupExist = await _unitOfWork.OrderGroupRepository.GetByIdAsync(id.ToString());
			if ( orderGroupExist != null )
			{
				//switch ( orderGroupExist.Status )
				//{
				//	case BaseStatus.Available:
				//		orderGroupExist.Status = BaseStatus.UnAvailable;
				//		break;
				//	case BaseStatus.UnAvailable:
				//		orderGroupExist.Status = BaseStatus.Available;
				//		break;
				//}
				orderGroupExist.Status = model.Status;
				var updateResult = await _unitOfWork.OrderGroupRepository.UpdateAsync(orderGroupExist);
				if ( updateResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Chuyển trạng thái (" + orderGroupExist.Status + ") thành công";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Chuyển trạng thái không thành công";
					return result;
				}
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Order group không tồn tại";
				return result;
			}
		}
		#region Cluster old
		//public async Task<ResponseObject<List<OrderGroupsResponse>>> OrderGroupClusterAsync()
		//{
		//	var result = new ResponseObject<List<OrderGroupsResponse>>();

		//	//lấy các order có status đang là processing ra để gom cụm
		//	//xác định số K để dùng thuật toán k-means - sử dụng số lượng order group đã có vì mỗi khi tạo order roup thì đã gán cho 1 shipper
		//	//tại vì số K đc lấy bằng số orderGroup nên nếu K lớn hơn số lượng order sẽ bị lỗi 
		//	//gán các điểm(order) vào mỗi cụm 

		//	//lấy ra các order có status đang processing
		//	var orders = _unitOfWork.OrderRepository.Get(x => x.Status == OrderStatus.Processing).ToList();
		//	if ( orders.Count <= 0 )
		//	{
		//		result.StatusCode = 400;
		//		result.Message = "Không có đơn hàng phù hợp. Không thể gom cụm";
		//		return result;
		//	}
		//	var orderGroups = _unitOfWork.OrderGroupRepository.Get(x => x.Status == BaseStatus.Available).ToList();

		//	#region orders < ordergroups
		//	//if ( orders.Count < orderGroups.Count )
		//	//{
		//		foreach ( var order in orders )
		//		{
		//			OrderGroup nearestOrderGroup = null;
		//			double minDistance = double.MaxValue;

		//			foreach ( var orderGroup in orderGroups )
		//			{
		//				double[] orderGroupCoordinates = { orderGroup.Longitude , orderGroup.Latitude };
		//				double[] orderCoordinates = { order.Longitude , order.Latitude };
		//				double distance = CalculateDistance(orderGroupCoordinates , orderCoordinates);

		//				if ( distance < minDistance )
		//				{
		//					nearestOrderGroup = orderGroup;
		//					minDistance = distance;
		//				}
		//			}

		//			if ( nearestOrderGroup != null && nearestOrderGroup.Orders == null )
		//			{
		//				nearestOrderGroup.Orders = new List<Order>();
		//				nearestOrderGroup.Orders.Add(order);
		//				order.OrderGroup = nearestOrderGroup;
		//				order.OrderGroupId = nearestOrderGroup.Id;
		//			}
		//			else if ( nearestOrderGroup != null && nearestOrderGroup.Orders != null )
		//			{
		//				nearestOrderGroup.Orders.Add(order);
		//				order.OrderGroup = nearestOrderGroup;
		//				order.OrderGroupId = nearestOrderGroup.Id;
		//			}
		//		}
		//		ChangeConfirmOrder(orders);
		//		result.StatusCode = 200;
		//		result.Message = "Gom cụm thành công";//Orders assigned to order groups successfully.
		//              result.Data = _mapper.Map<List<OrderGroupsResponse>>(orderGroups);
		//		return result;
		//	//}
		//	#endregion

		//	////gọi kmeans
		//	////var k = FindOptimalK(orders , orderGroups.Count);
		//	//var kmeans = new KMeans(6 , new SquareEuclidean());//SquareEuclidean được sử dụng để chỉ rằng chúng ta muốn tính
		//	//												   //khoảng cách bình phương giữa các điểm trong quá trình gom cụm

		//	////lấy các cặp kinh độ, vĩ độ từ orders
		//	//var coordinates = orders.Select(o => new double[] { o.Longitude , o.Latitude }).ToArray();//trả về 1 mảng double[]
		//	//var clusters = kmeans.Learn(coordinates);

		//	////lấy ra tâm cụm
		//	//var centroids = clusters.Centroids;

		//	//// Tạo một mảng để lưu chỉ số cụm của mỗi đơn hàng
		//	//var orderClusterIndices = new int[orders.Count];

		//	//// Xác định chỉ số cụm cho mỗi đơn hàng
		//	//for ( int i = 0; i < orders.Count; i++ )
		//	//{
		//	//	var order = orders[i];
		//	//	var point = new double[] { order.Longitude , order.Latitude };
		//	//	orderClusterIndices[i] = GetClosestCentroidIndex(point , centroids);
		//	//}

		//	//// Gán các đơn hàng vào OrderGroup dựa trên chỉ số cụm
		//	//foreach ( var orderGroup in orderGroups )
		//	//{
		//	//	var clusterIndex = orderGroups.IndexOf(orderGroup);
		//	//	var groupedOrders = orders.Where((o , index) => orderClusterIndices[index] == clusterIndex).ToList();

		//	//	// Cập nhật Orders cho OrderGroup hiện tại
		//	//	orderGroup.Orders = groupedOrders;
		//	//}
		//	//// Cập nhật các OrderGroup vào cơ sở dữ liệu
		//	//var clusterResult = await _unitOfWork.OrderGroupRepository.UpdateRangeAsync(orderGroups);
		//	//if ( clusterResult )
		//	//{
		//	//	ChangeConfirmOrder(orders);
		//	//	result.StatusCode = 200;
		//	//	result.Message = "Gán đơn hàng thành công";//Orders assigned to order groups successfully.
		// //             result.Data = _mapper.Map<List<OrderGroupsResponse>>(orderGroups);
		//	//	return result;
		//	//}
		//	//result.StatusCode = 400;
		//	//result.Message = "Gán đơn hàng không thành công";//Orders assigned to order groups unsuccessfully!
		// //         return result;
		//}

		//// Hàm tính khoảng cách Euclidean giữa hai điểm
		//private double CalculateEuclideanDistance(double[] point1 , double[] point2)
		//{
		//	double sum = 0;
		//	for ( int i = 0; i < point1.Length; i++ )
		//	{
		//		sum += Math.Pow(point1[i] - point2[i] , 2);
		//	}
		//	return Math.Sqrt(sum);
		//}

		//public int FindOptimalK(List<Order> orders , int maxK)
		//{
		//	var coordinates = orders.Select(o => new double[] { o.Longitude , o.Latitude }).ToArray();
		//	var sseList = new List<double>();

		//	for ( int k = 1; k <= maxK; k++ )
		//	{
		//		var kmeans = new KMeans(k , new SquareEuclidean());
		//		var clusters = kmeans.Learn(coordinates);

		//		// Tính tổng bình phương khoảng cách (SSE) của các điểm đến tâm cụm gần nhất
		//		var sse = coordinates.Sum(point =>
		//		{
		//			var clusterIndex = GetClosestCentroidIndex(point , clusters.Centroids);
		//			var distance = CalculateEuclideanDistance(point , clusters.Centroids[clusterIndex]);
		//			return distance * distance;
		//		});

		//		sseList.Add(sse);
		//	}

		//	// Tìm điểm gấp khúc trong danh sách SSE
		//	int optimalK = DetermineElbow(sseList);
		//	return 6;
		//}

		//// Hàm để lấy chỉ số của tâm cụm gần nhất
		//private int GetClosestCentroidIndex(double[] point , double[][] centroids)
		//{
		//	int closestIndex = -1;
		//	double minDistance = double.MaxValue;

		//	for ( int i = 0; i < centroids.Length; i++ )
		//	{
		//		var distance = CalculateEuclideanDistance(point , centroids[i]);
		//		if ( distance < minDistance )
		//		{
		//			minDistance = distance;
		//			closestIndex = i;
		//		}
		//	}

		//	return closestIndex;
		//}
		//private int DetermineElbow(List<double> sseList)
		//{
		//	//if ( sseList.Count < 2 ) return 1; // Không đủ dữ liệu để xác định

		//	// Tính độ dốc giữa các điểm SSE
		//	var changes = new List<double>();
		//	for ( int i = 1; i < sseList.Count; i++ )
		//	{
		//		double change = sseList[i - 1] - sseList[i];
		//		changes.Add(change);
		//	}

		//	// Tìm sự thay đổi lớn nhất trong độ dốc
		//	var maxChange = changes.Select((c , i) => new { Index = i , Change = c }).OrderByDescending(x => x.Change).First();
		//	return maxChange.Index + 2; // +2 vì index bắt đầu từ 0 và k bắt đầu từ 1
		//}

		//#region Old
		//private static double CalculateDistance(double[] point1 , double[] point2)
		//{
		//	var lat1 = point1[0] * (Math.PI / 180.0);
		//	var lon1 = point1[1] * (Math.PI / 180.0);
		//	var lat2 = point2[0] * (Math.PI / 180.0);
		//	var lon2 = point2[1] * (Math.PI / 180.0);
		//	var dLat = lat2 - lat1;
		//	var dLon = lon2 - lon1;

		//	var a = Math.Pow(Math.Sin(dLat / 2.0) , 2.0) +
		//			Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon / 2.0) , 2.0);
		//	var c = 2.0 * Math.Atan2(Math.Sqrt(a) , Math.Sqrt(1.0 - a));

		//	return 6371.0 * c; // return distance in kilometers
		//}

		//public int Decision(double[] point , double[][] centroids)
		//{
		//	int closestIndex = -1;
		//	double minDistance = double.MaxValue;

		//	for ( int i = 0; i < centroids.Length; i++ )
		//	{
		//		var distance = CalculateEuclideanDistance(point , centroids[i]);
		//		if ( distance < minDistance )
		//		{
		//			minDistance = distance;
		//			closestIndex = i;
		//		}
		//	}

		//	return closestIndex;
		//}
		//#endregion

		#endregion
		public async Task<ResponseObject<List<OrderGroupsResponse>>> OrderGroupClusterAsync()
		{
			var result = new ResponseObject<List<OrderGroupsResponse>>();

			// Lấy các order có status là processing để gom cụm
			var orders = _unitOfWork.OrderRepository.Get(x => x.Status == OrderStatus.Processing).ToList();
			if ( orders.Count <= 0 )
			{
				result.StatusCode = 400;
				result.Message = "Không có đơn hàng phù hợp. Không thể gom cụm";
				return result;
			}

			// Lấy các order group có sẵn
			var orderGroups = _unitOfWork.OrderGroupRepository.Get(x => x.Status == BaseStatus.Available).ToList();

			// Nếu không có OrderGroup phù hợp
			if ( orderGroups.Count <= 0 )
			{
				result.StatusCode = 400;
				result.Message = "Không có nhóm đơn hàng phù hợp để gom cụm.";
				return result;
			}

			// Gom cụm các order vào nhóm orderGroup gần nhất
			foreach ( var order in orders )
			{
				OrderGroup nearestOrderGroup = null;
				double minDistance = double.MaxValue;

				// Tìm OrderGroup có khoảng cách nhỏ nhất với Order hiện tại
				foreach ( var orderGroup in orderGroups )
				{
					double distance = CalculateDistance(
						new double[] { orderGroup.Longitude , orderGroup.Latitude } ,
						new double[] { order.Longitude , order.Latitude }
					);

					if ( distance < minDistance )
					{
						nearestOrderGroup = orderGroup;
						minDistance = distance;
					}
				}

				// Gán order vào OrderGroup gần nhất
				if ( nearestOrderGroup != null )
				{
					if ( nearestOrderGroup.Orders == null )
					{
						nearestOrderGroup.Orders = new List<Order>();
					}
					nearestOrderGroup.Orders.Add(order);
					order.OrderGroup = nearestOrderGroup;
					order.OrderGroupId = nearestOrderGroup.Id;
				}
			}

			// Xác nhận thay đổi của các đơn hàng sau khi gom cụm
			ChangeConfirmOrder(orders);

			// Cập nhật dữ liệu vào cơ sở dữ liệu
			var updateResult = await _unitOfWork.OrderGroupRepository.UpdateRangeAsync(orderGroups);
			if ( updateResult )
			{
				result.StatusCode = 200;
				result.Message = "Gom cụm thành công";
				result.Data = _mapper.Map<List<OrderGroupsResponse>>(orderGroups);
			}
			else
			{
				result.StatusCode = 400;
				result.Message = "Gom cụm không thành công";
			}

			return result;
		}

		// Hàm tính khoảng cách Euclidean giữa hai điểm
		private double CalculateDistance(double[] point1 , double[] point2)
		{
			double sum = 0;
			for ( int i = 0; i < point1.Length; i++ )
			{
				sum += Math.Pow(point1[i] - point2[i] , 2);
			}
			return Math.Sqrt(sum);
		}     

		//Hàm để thay đổi confirm order
		private async void ChangeConfirmOrder(List<Order> orders)
		{
			foreach ( var order in orders )
			{
				if ( order.Transaction != null
					&& ((order.Transaction.Type == TransactionType.ZaloPay && order.Transaction.Status == TransactionStatus.PAID)
						|| (order.Transaction.Type == TransactionType.COD)) )
				{
					order.Status = OrderStatus.Confirm;
				}
			}
			await _unitOfWork.CompleteAsync();
		}
	}
}
