using Microsoft.EntityFrameworkCore.Query;
using Pager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Repository.Interface
{
    public interface IRepository<TEntity> where TEntity : class
    {
        #region Sync 
        void Add(TEntity entity);
        void Add(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        void Delete(object id);
        void Delete(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Update(IEnumerable<TEntity> entities);
        void Delete(Expression<Func<TEntity, bool>> where);

        int Count();
        /// <summary>
        ///  Finds an entity with the given primary key values. If an entity with the given
        ///     primary key values is being tracked by the context, then it is returned immediately
        ///     without making a request to the database. Otherwise, a query is made to the database
        ///     for an entity with the given primary key values and this entity, if found, is
        ///     attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        TEntity GetById(params object[] keyValues);
        TEntity Single(Expression<Func<TEntity, bool>> predicate = null);
        TEntity Single(Expression<Func<TEntity, bool>> predicate = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null,
           bool disableTracking = true);

        IPaginate<TEntity> GetPaginatedList(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null, int index = 0, int size = 20,
            bool disableTracking = true);
        IPaginate<TResult> GetPaginatedList<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null, int index = 0, int size = 20,
            bool disableTracking = true) where TResult : class;

        IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null);

        IEnumerable<TResult> GetList<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null) where TResult : class;


        // execute custom sql and return single object     
        TEntity ExecuteSingle(string sql, params object[] param);
        // execute custom sql and return single object without parameter
        TEntity ExecuteSingle(string sql);
        // execute custom sql and return multiple objects without parameter
        IEnumerable<TEntity> ExecuteList(string sql);
        // execute custom sql and return multiple objects with parameter
        IEnumerable<TEntity> ExecuteList(string sql, params object[] param);

        #endregion

        #region Async

        Task AddAsync(TEntity entity);
        Task AddAsync(IEnumerable<TEntity> entities);

        Task<int> CountAsync();
        
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null,
           bool disableTracking = true);

        Task<IPaginate<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null, int index = 0, int size = 20,
            bool disableTracking = true);

        Task<IPaginate<TResult>> GetPaginatedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null, int index = 0, int size = 20,
            bool disableTracking = true) where TResult : class;

        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null);

        Task<IEnumerable<TResult>> GetListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null) where TResult : class;

        Task<TEntity> ExecuteSingleAsync(string sql, params object[] param);
        Task<TEntity> ExecuteSingleAsync(string sql);
        Task<IEnumerable<TEntity>> ExecuteListAsync(string sql);
        Task<IEnumerable<TEntity>> ExecuteListAsync(string sql, params object[] param);
        #endregion

    }
}
