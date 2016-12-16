using System;
using System.Collections.Generic;

namespace Presence.Core.Interfaces.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        TEntity Get(object id);
        TEntity Update(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        TEntity Insert(TEntity entity);
    }
}
