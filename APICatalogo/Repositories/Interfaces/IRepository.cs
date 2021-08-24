using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace APICatalogo.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        Task<List<T>> Get();
        Task<T> GetById(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
