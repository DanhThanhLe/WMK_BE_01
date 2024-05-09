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
        private readonly DbSet<T> _dbSet;
        public BaseRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public void Create1(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public bool Create2(T entity)
        {
            _context.Add(entity);
            var result = _context.SaveChanges();
            return result > 0;
        }

        public void Delete1(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public bool Delete2(T entity)
        {
            _dbSet.Remove(entity);
            var result = _context.SaveChanges();
            return result > 0;
        }

        public T GetById<TKey>(TKey id)
        {
            return _dbSet.Find(new object[1] { id });
        }

        public void Update1(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public bool Update2(T entity)
        {
            _dbSet.Update(entity);
            var result = _context.SaveChanges();
            return result > 0;
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }

        public void DetachEntity(T entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }
        public IQueryable<T> GetAll() 
        {
            return _dbSet;
        }

        public T GetById(string id)
        {
            try
            {
                Guid guidId = Guid.Parse(id);
                if (_dbSet.Find(guidId) is var found && found != null)
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
