using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Infrastructure.Helper
{
    public class CheckInstanceOfTEntityClass<TEntity> : ICheckInstanceOfTEntityClass<TEntity> where TEntity : class, TEntityClass
    {
        public TEntity CheckNullOrNot(TEntity? entity, string name)
        {
            if (entity is null)
                throw new NotFoundException($"{name} not found!");
            return entity;
        }
    }
}