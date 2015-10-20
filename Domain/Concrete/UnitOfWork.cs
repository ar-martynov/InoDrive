using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

using Domain.Abstract;
using Domain.Abstract.EntityRepositories;
using Domain.Helpers;
using Domain.Concrete.Repositories;
using Domain.Contexts;

namespace Domain.Concrete
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly InoDriveContext _context;
        private Dictionary<Type, object> _repositories;

        public UnitOfWork()
        {
            this._context = new InoDriveContext();
        }

        public UnitOfWork(InoDriveContext context)
        {
            this._context = context;
            _context.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="TRepository">The type of the repository.</typeparam>
        /// <returns></returns>
        public TRepository GetRepository<TRepository>()
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();

            if (_repositories.Keys.Contains(typeof(TRepository)))
                return (TRepository)_repositories[typeof(TRepository)];

            var repository = NinjectResolver.kernel.Get<TRepository>(new Ninject.Parameters.ConstructorArgument("context", _context));
            _repositories.Add(typeof(TRepository), repository);
            return repository;
        }


        /// <summary>
        /// Commits this instance.
        /// </summary>
        public void Commit()
        {
            _context.SaveChanges();
        }

        /// <summary>
        /// Commits the asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_context != null)
            {
                _repositories.Clear();
                _context.Dispose();
            }
        }
    }
}
