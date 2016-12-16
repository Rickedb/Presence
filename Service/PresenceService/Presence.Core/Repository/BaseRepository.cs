using Presence.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Presence.Core.Repository
{
    public class BaseRepository<TEntity> : IDisposable, IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        protected DbSet<TEntity> DbSet;

        public BaseRepository(DbContext context)
        {
            this._context = context;
            this.DbSet = this._context.Set<TEntity>();
        }

        public virtual void Delete(TEntity entity)
        {
            var entry = this._context.Entry(entity);
            if (entry.State == EntityState.Detached)
                this.DbSet.Attach(entity);
            this.DbSet.Remove(entity);
            this._context.SaveChanges();
        }

        public virtual void Delete(object id)
        {
            TEntity entity = (TEntity)Activator.CreateInstance(typeof(TEntity), new object[] { id });
            this.DbSet.Remove(entity);
        }

        public void Dispose()
        {
            this._context.Dispose();
            GC.SuppressFinalize(this);
        }

        public virtual TEntity Get(object id)
        {
            return this.DbSet.Find(id);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return this.DbSet.ToList();
        }

        public virtual TEntity Insert(TEntity entity)
        {
            var entry = _context.Entry(entity);
            DbSet.Attach(entity);
            entry.State = EntityState.Added;
            this._context.SaveChanges();

            return entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            var entry = this._context.Entry(entity);
            entry.State = EntityState.Modified;
            this._context.SaveChanges();

            return entity;
        }
    }
}
