using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Abstract
{
    public interface IRepository<TObject> where TObject : class
    {
        TObject Get(int id);
        Task<TObject> GetAsync(int id);
        TObject Find(Expression<Func<TObject, bool>> match);
        Task<TObject> FindAsync(Expression<Func<TObject, bool>> match);
        bool Update(TObject updated, int key);
        Task<bool> UpdateAsync(TObject updated, int key);
        void Delete(TObject t);
        int Count();
        Task<int> CountAsync();
    }
}
