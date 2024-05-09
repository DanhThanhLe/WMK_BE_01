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
        public T GetById<TKey>(TKey id);
        public IQueryable<T> GetAll();
        //public void Insert (T entity);
        public void Create1(T entity);
        public void Update1(T entity);
        public void Delete1(T entity);
        public IQueryable<T> Get(Expression<Func<T, bool>> expression);
        public void DetachEntity(T entity);
        public bool Create2(T entity);
        public bool Update2(T entity);
        public bool Delete2(T entity);
        public T GetById(string id);
        public T GetEntity(T entity);
    }
}
