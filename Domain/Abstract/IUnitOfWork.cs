using System;
using System.Threading.Tasks;
using Domain.Abstract.EntityRepositories;

namespace Domain.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        TRepository GetRepository<TRepository>();
        void Commit();
        Task CommitAsync();
    }
}
