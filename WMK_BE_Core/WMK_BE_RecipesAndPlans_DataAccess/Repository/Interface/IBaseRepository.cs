using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface
{
    public interface IBaseRepository<T> where T : class
    {
		Task<List<T>> GetAllAsync();
        public IQueryable<T> GetAll();
        Task<T?> GetByIdAsync(string id);
		//public void Create(T entity);
		Task<bool> CreateAsync(T entity);
		//public void Update(T entity);
		Task<bool> UpdateAsync(T entity);
		//public void Delete(string id);
		Task<bool> DeleteAsync(string id);
		public T GetById<TKey>(TKey id);
        public void DetachEntity(T entity);
        public IQueryable<T> Get(Expression<Func<T, bool>> expression);
        public T GetEntity(T entity);
    }
}
