using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Infrastructure.Persistence.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet;
        public Repository(DbSet<TEntity> dbset)
        {
            _dbSet = dbset;
        }
        public void Add(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _dbSet.AddRange(entities);
        }

        public async Task<TEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(id, cancellationToken);
        }

        public void Remove(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}