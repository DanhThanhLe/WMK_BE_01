using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
	public class OrderGroupRepository : BaseRepository<OrderGroup>, IOrderGroupRepository
	{
		public OrderGroupRepository(WeMealKitContext context) : base(context)
		{

		}

		public bool OrderGroupExistOrder(OrderGroup orderGroup)
		{
			var result = _dbSet.Include(og => og.Orders).FirstOrDefault();
			if ( result != null && result.Orders.Count > 0 )
			{
				return true;
			}
			return false;
		}

		public override async Task<List<OrderGroup>> GetAllAsync()
		{
			var orderGroupList = await _dbSet
				.Include(od => od.Orders)
				.ThenInclude(od => od.User)
				.ToListAsync();
			return orderGroupList;
		}

		public override async Task<OrderGroup?> GetByIdAsync(string? id)
		{
			try
			{
				Guid guidId;
				if ( !Guid.TryParse(id , out guidId) )
				{
					return null;
				}
				var order = await _dbSet
					.Include(od => od.Orders)
					.Include(od => od.User)
					.FirstOrDefaultAsync(r => r.Id == guidId);
				return order;
			}
			catch ( Exception ex )
			{
				Console.WriteLine($"Error occurred in GetByIdAsync: {ex}");
				return null;
			}
		}

		public async Task<bool> UpdateRangeAsync(List<OrderGroup> orderGroups)
		{
			if ( orderGroups == null || !orderGroups.Any() )
			{
				return false; // Không có gì để cập nhật
			}

			try
			{
				// Lấy danh sách các ID của các OrderGroup để cập nhật
				var orderGroupIds = orderGroups.Select(og => og.Id).ToList();

				// Tải các OrderGroup hiện tại từ cơ sở dữ liệu dựa trên ID
				var existingOrderGroups = await _dbSet
					.Include(og => og.Orders) // Đảm bảo tải danh sách đơn hàng liên quan
					.Where(og => orderGroupIds.Contains(og.Id))
					.ToListAsync();

				foreach ( var newOrderGroup in orderGroups )
				{

					// Gán các Order mới từ newOrderGroup cho OrderGroup hiện tại
					if ( newOrderGroup.Orders != null )
					{
						//update order group
						_dbSet.Update(newOrderGroup);
					}
				}

				return true;
			}
			catch ( Exception ex )
			{
				// Xử lý lỗi (log hoặc thông báo)
				Console.WriteLine($"Error occurred in UpdateRangeAsync: {ex}");
				return false;
			}
		}



	}
}
