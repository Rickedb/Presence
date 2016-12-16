using Presence.Core.Interfaces.Repository;
using Presence.Core.Interfaces.Services;
using System.Collections.Generic;
using System;

namespace Presence.Core.Services
{
    public class BaseServices<TEntity> : IBaseServices<TEntity> where TEntity : class
    {
        private readonly IBaseRepository<TEntity> _repository;

        public BaseServices(IBaseRepository<TEntity> repository)
        {
            this._repository = repository;
        }

        public void Delete(TEntity entity)
        {
            this._repository.Delete(entity);
        }

        public void Delete(object id)
        {
            this._repository.Delete(id);
        }

        public TEntity Get(object id)
        {
            return this._repository.Get(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return this._repository.GetAll();
        }

        public TEntity Insert(TEntity entity)
        {
            return this._repository.Insert(entity);
        }

        public TEntity Update(TEntity entity)
        {
            return this._repository.Update(entity);
        }
    }
}
