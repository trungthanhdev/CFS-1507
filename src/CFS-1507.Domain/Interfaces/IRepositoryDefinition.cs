using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;

namespace CFS_1507.Domain.Interfaces
{
    public interface IRepositoryDefinition<TEntity> where TEntity : TEntityClass
    {
        void Add(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
        void Remove(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
        Task<TEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken));
    }
}