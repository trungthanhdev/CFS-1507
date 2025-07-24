using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;

namespace CFS_1507.Domain.Interfaces
{
    public interface ICheckInstanceOfTEntityClass<TEntity> where TEntity : TEntityClass
    {
        TEntity CheckNullOrNot(TEntity? entity, string name);
    }
}