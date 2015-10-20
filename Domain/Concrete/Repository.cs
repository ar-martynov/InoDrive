using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Domain.Abstract;
using Domain.Contexts;

namespace Domain.Concrete
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected  InoDriveContext _context;

        public Repository(InoDriveContext dbContext)
        {
            this._context = dbContext;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public T Get(int id)
        {
           return _context.Set<T>().Find(id);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<T> GetAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// Finds the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        public T Find(Expression<Func<T, bool>> match)
        {
            return _context.Set<T>().SingleOrDefault(match);
        }

        /// <summary>
        /// Finds the asynchronous.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        public async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(match);
        }


        /// <summary>
        /// Updates the specified updated.
        /// </summary>
        /// <param name="updated">The updated.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool Update(T updated, int key)
        {
            if (updated == null)
                return false;
 
            T existing = _context.Set<T>().Find(key);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(updated);
            }
            return true;
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="updated">The updated.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(T updated, int key)
        {
            if (updated == null)
                return false;
 
            T existing = await _context.Set<T>().FindAsync(key);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(updated);
            }
            return true;
        }

        /// <summary>
        /// Deletes the specified t.
        /// </summary>
        /// <param name="t">The t.</param>
        public void Delete(T t)
        {
            _context.Set<T>().Remove(t);
        }


        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return _context.Set<T>().Count();
        }

        /// <summary>
        /// Counts the asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountAsync()
        {
          return await _context.Set<T>().CountAsync();
        }
    }
}
