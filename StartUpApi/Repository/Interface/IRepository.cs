using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StartUpApi.Repository.Interface
{
    public interface IRepository<TEntity> where TEntity : class
    {
        // Marks an entity as new
        void Add(TEntity entity);
        // Marks an entity as modified
        void Update(TEntity entity);
        // Marks an entity to be removed
        void Delete(TEntity entity);
        void Delete(Expression<Func<TEntity, bool>> where);
        // Get an entity by id
        Task<TEntity> GetByIdAsync(string id);       
        // Get an entity count
        Task<int> CountAsync();
        // Get an entity using delegate
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where);
        // Gets all entities of type T
        Task<IEnumerable<TEntity>> GetAllAsync();
        // Gets entities using delegate
        Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> where);
    }
}
