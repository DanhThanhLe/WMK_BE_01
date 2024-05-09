using Microsoft.EntityFrameworkCore;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly DbContext _context;
        internal DbSet<T> _dbSet { get; set; }
        public BaseRepository(DbContext context)
        {
            this._context = context;
            this._dbSet = this._context.Set<T>();
        }

		public void Create(T entity)
		{
			throw new NotImplementedException();
		}
		public void Update(T entity)
		{
			throw new NotImplementedException();
		}
		public void Delete(string id)
		{
			throw new NotImplementedException();
		}
		public virtual Task<List<T>> GetAllAsync()
		{
			return this._dbSet.ToListAsync();
		}
		public virtual async Task<T?> GetByIdAsync(string id)
		{
			try
			{
				Guid guidId;
				if ( !Guid.TryParse(id , out guidId) )
				{
					return null;
				}
				var entity = await _dbSet.FindAsync(guidId);
				return entity;
			}
			catch ( Exception ex )
			{
				Console.WriteLine($"Error occurred in GetAsync: {ex}");
				return null;
			}
		}
		public virtual async Task<bool> CreateAsync(T entity)
		{
			try
			{
				await _dbSet.AddAsync(entity);
				return true;
			}
			catch ( Exception ex )
			{
				Console.WriteLine($"Error occurred in CreateAsync: {ex}");
				return false;
			}
		}
		public virtual Task<bool> UpdateAsync(T entity)
		{
			try
			{
				_dbSet.Update(entity);
				return Task.FromResult(true);
			}
			catch ( Exception ex )
			{
				Console.WriteLine($"Error occurred in UpdateAsync: {ex}");
				return Task.FromResult(false);
			}
		}
		public virtual async Task<bool> DeleteAsync(string id)
		{
			try
			{
				var entity = await _dbSet.FindAsync(id);
				if ( entity != null )
				{
					_context.Set<T>().Remove(entity);
					return true;
				}
				else
				{
					return false;
				}
			}
			catch ( Exception ex )
			{
				Console.WriteLine($"Error occurred in DeleteAsync: {ex}");
				return false;
			}
		}

		public T GetById<TKey>(TKey id)
        {
            return _dbSet.Find(new object[1] { id });
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }

        public void DetachEntity(T entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }
        public T GetEntity(T entity)
        {
            try
            {
                if (_dbSet.Find(entity) is var found && found != null)
                {
                    return found;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
