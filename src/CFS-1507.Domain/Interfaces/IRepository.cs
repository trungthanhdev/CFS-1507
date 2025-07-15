using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Domain.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Add(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
        void Remove(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
        Task<TEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken));
    }
}