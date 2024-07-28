using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
	public class OrderRepository : BaseRepository<Order>, IOrderRepository
	{
        public OrderRepository(WeMealKitContext context) : base(context)
        {
            
        }

		public async Task<bool> GetUserExistInOrderAsync(Guid idOrder , Guid userId)
		{
			var userExist = await _dbSet.Include(u => u.User).FirstOrDefaultAsync(o => o.Id == idOrder && o.UserId == userId);
			if ( userExist != null && userExist.User != null )
			{
				return true;
			}
			return false;
		}

		public override async Task<List<Order>> GetAllAsync()
		{
			return await _dbSet
                .Include(o => o.Transactions)
                .Include(o => o.OrderDetails)
					.ThenInclude(od => od.Recipe)
				.Include(o => o.OrderDetails)
					.ThenInclude(od => od.RecipeIngredientOrderDetails)
						.ThenInclude(ri => ri.Ingredient)
				.ToListAsync();
		}

		public override async Task<Order> GetByIdAsync(string id)
		{
            try
            {
                Guid guidId;
                if (!Guid.TryParse(id, out guidId))
                {
                    return null;
                }
                var order = await _dbSet
                    .Include(o => o.Transactions)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Recipe)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.RecipeIngredientOrderDetails)
                            .ThenInclude(ri => ri.Ingredient)
                    .FirstOrDefaultAsync(r => r.Id == guidId);
                return order;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred in GetByIdAsync: {ex}");
                return null;
            }
        }

        public override IQueryable<Order> Get(Expression<Func<Order, bool>> expression)
        {
            return _dbSet
                .Where(expression)
                .Include(o => o.Transactions)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Recipe)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.RecipeIngredientOrderDetails)
                        .ThenInclude(ri => ri.Ingredient);
        }

    }
}
