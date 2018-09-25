using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Pager.Interface;
using System.Reflection;
using Pager.Infrastructure;
using Data.Repository.Interface;

namespace Data.Repository.Infrastructure
{
    public class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public RepositoryBase(DbContext context)
        {
            _dbContext = context ?? throw new ArgumentException(nameof(context));
            _dbSet = _dbContext.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void Add(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public int Count()
        {
            return _dbSet.Count();
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public void Delete(TEntity existing)
        {
            if (existing != null) _dbSet.Remove(existing);
        }

        public void Delete(object id)
        {
            var typeInfo = typeof(TEntity).GetTypeInfo();
            var key = _dbContext.Model.FindEntityType(typeInfo.Name).FindPrimaryKey().Properties.FirstOrDefault();
            var property = typeInfo.GetProperty(key?.Name);
            if (property != null)
            {
                var entity = Activator.CreateInstance<TEntity>();
                property.SetValue(entity, id);
                _dbContext.Entry(entity).State = EntityState.Deleted;
            }
            else
            {
                var entity = _dbSet.Find(id);
                if (entity != null) Delete(entity);
            }
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = _dbSet.Where(predicate).ToList();
            entities.ForEach(e => _dbSet.Remove(e));
        }

        public IEnumerable<TEntity> ExecuteList(string sql)
        {
            return _dbSet.FromSql(sql).ToList();
        }

        public IEnumerable<TEntity> ExecuteList(string sql, params object[] param)
        {
            return _dbSet.FromSql(sql, param).ToList();
        }

        public async Task<IEnumerable<TEntity>> ExecuteListAsync(string sql)
        {
            return await _dbSet.FromSql(sql).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> ExecuteListAsync(string sql, params object[] param)
        {
            return await _dbSet.FromSql(sql, param).ToListAsync();
        }

        public TEntity ExecuteSingle(string sql, params object[] param)
        {
            return _dbSet.FromSql(sql, param).FirstOrDefault();
        }

        public TEntity ExecuteSingle(string sql)
        {
            return _dbSet.FromSql(sql).FirstOrDefault();
        }

        public async Task<TEntity> ExecuteSingleAsync(string sql, params object[] param)
        {
            return await _dbSet.FromSql(sql, param).FirstOrDefaultAsync();
        }

        public async Task<TEntity> ExecuteSingleAsync(string sql)
        {
            return await _dbSet.FromSql(sql).FirstOrDefaultAsync();
        }

        /// <summary>
        ///  Finds an entity with the given primary key values. If an entity with the given
        ///     primary key values is being tracked by the context, then it is returned immediately
        ///     without making a request to the database. Otherwise, a query is made to the database
        ///     for an entity with the given primary key values and this entity, if found, is
        ///     attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public TEntity GetById(params object[] keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public IPaginate<TEntity> GetPaginatedList(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null, int index = 0, int size = 20, bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (includes != null && includes.Count > 0)
            {
                foreach (var include in includes)
                    query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return orderBy != null ? orderBy(query).ToPaginate(index, size) : query.ToPaginate(index, size);
        }

        public IPaginate<TResult> GetPaginatedList<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, 
            Func<IQueryable<TResult>,            IOrderedQueryable<TResult>> orderBy = null, List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null, int index = 0, int size = 20, bool disableTracking = true) where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (includes.Count > 0)
            {
                foreach (var include in includes)
                    query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var result = query.Select(selector);
            return orderBy != null ? orderBy(result).ToPaginate(index, size) : result.ToPaginate(index, size);
        }

        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Count > 0)
            {
                foreach (var include in includes)
                    query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return (orderBy != null ? orderBy(query) : query).ToList();
        }

        public IEnumerable<TResult> GetList<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, 
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null, List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null) where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Count > 0)
            {
                foreach (var include in includes)
                    query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            var result = query.Select(selector);
            return orderBy != null ? orderBy(result).ToList() : result.ToList();
        }

        public async Task<IPaginate<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null, int index = 0, int size = 20, bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (includes != null && includes.Count > 0)
            {
                foreach (var include in includes)
                    query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await (orderBy != null ? orderBy(query).ToPaginateAsync(index, size) : query.ToPaginateAsync(index, size));
        }

        public async Task<IPaginate<TResult>> GetPaginatedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null, List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null, int index = 0, int size = 20, bool disableTracking = true) where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (includes != null && includes.Count > 0)
            {
                foreach (var include in includes)
                    query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            //var query2 = query;
            //var hmm = orderBy != null ? orderBy(query2).Select(selector) : query2.Select(selector);
            //var resulte = hmm.ToSql();
            var result = query.Select(selector);
            return await (orderBy != null ? orderBy(result).ToPaginateAsync(index, size) : result.ToPaginateAsync(index, size));
        }

        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Count > 0)
            {
                foreach (var include in includes)
                    query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await (orderBy != null ? orderBy(query).ToListAsync() : query.ToListAsync());
        }

        public async Task<IEnumerable<TResult>> GetListAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, 
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null, List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null) where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Count > 0)
            {
                foreach (var include in includes)
                    query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            var result = query.Select(selector);
            return await (orderBy != null ? orderBy(result).ToListAsync() : result.ToListAsync());
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);

            return query.FirstOrDefault();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null, bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking) query = query.AsNoTracking();

            if (includes != null && includes.Count > 0)
            {
                foreach (var include in includes)
                    query = include(query);
            }

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return orderBy(query).FirstOrDefault();
            return query.FirstOrDefault();
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null, bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking) query = query.AsNoTracking();

            if (includes != null && includes.Count > 0)
            {
                foreach (var include in includes)
                    query = include(query);
            }

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return orderBy(query).FirstOrDefault();
            return await query.FirstOrDefaultAsync();
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }
    }
}
