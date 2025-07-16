using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;
using CFS_1507.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Infrastructure.Persistence.Repositories
{
    public class RepositoryDefinition<TEntity> : IRepositoryDefinition<TEntity> where TEntity : class, TEntityClass
    {
        private readonly DbSet<TEntity> _dbSet;
        private readonly AppDbContext _context;
        public RepositoryDefinition(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
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