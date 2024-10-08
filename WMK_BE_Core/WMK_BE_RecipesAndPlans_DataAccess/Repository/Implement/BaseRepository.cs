﻿using Microsoft.EntityFrameworkCore;
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

		public IQueryable<T> GetAll()
		{
			return _dbSet;
		}

		//      public void Create(T entity)
		//{
		//	throw new NotImplementedException();
		//}
		//public void Update(T entity)
		//{
		//	throw new NotImplementedException();
		//}
		public void Delete(string id)
		{
			var entity = _dbSet.Find(id);
			if ( entity != null )
			{
				this._dbSet.Remove(entity);
				_context.SaveChanges();
			}
		}
		public virtual Task<List<T>> GetAllAsync()
		{
			return this._dbSet.ToListAsync();
		}
		public virtual async Task<T?> GetByIdAsync(string? id)
		{
			try
			{
				// Nếu id là null hoặc trống, trả về null
				if ( string.IsNullOrEmpty(id) )
				{
					return null;
				}

				Guid guidId;
				if ( Guid.TryParse(id , out guidId) )
				{
					// Nếu id là Guid, tìm kiếm theo Guid
					var entity = await _dbSet.FindAsync(guidId);
					return entity;
				}
				else
				{
					// Nếu id không phải Guid, tìm kiếm theo string
					var entity = await _dbSet.FindAsync(id);
					return entity;
				}
			}
			catch ( Exception ex )
			{
				Console.WriteLine($"Error occurred in GetByIdAsync: {ex}");
				return null;
			}
		}

		public virtual async Task<bool> CreateAsync(T entity)
		{
			try
			{
				_dbSet.Attach(entity);
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
				_dbSet.Attach(entity);
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
				Guid guidId;
				if ( !Guid.TryParse(id , out guidId) )
				{
					return false;
				}
				var entity = await _dbSet.FindAsync(guidId);
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

		public T? GetById(string id)
		{
			return _dbSet.Find(id);
		}

		public virtual IQueryable<T> Get(Expression<Func<T , bool>> expression)
		{
			return _dbSet.Where(expression);
		}

		public void DetachEntity(T entity)
		{
            var entry = _context.Entry(entity);
            if (entry != null)
            {
                entry.State = EntityState.Detached;
            }
        }
		public T GetEntity(T entity)
		{
			try
			{
				if ( _dbSet.Find(entity) is var found && found != null )
				{
					return found;
				}
				else
				{
					return null;
				}
			}
			catch ( Exception ex )
			{
				Console.WriteLine(ex.ToString());
				return null;
			}
		}
	}
}
