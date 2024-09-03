using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mod.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //T - Category
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null);
        T GetFirstOrDefault();
        IEnumerable<T> GetAll(string? includeProperties = null);
        int Count(Expression<Func<T, bool>> filter);
        IEnumerable<T> GetRange(Expression<Func<T, bool>> filder, int start, int end, string? includeProperties = null);
        IEnumerable<T> GetRange(int start, int end, string? includeProperties = null);
        IEnumerable<T> GetRange(int end, string? includeProperties = null);
        void Add(T entity);
        void Entry(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
        bool Any(Expression<Func<T, bool>> filter);
        abstract void Update(T entity);
    }
}
