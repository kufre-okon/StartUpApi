using Microsoft.EntityFrameworkCore;
using StartUpApi.Models;
using StartUpApi.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace StartUpApi.Repository.Infrastructure
{
    public abstract class RepositoryBase<TEntity> where TEntity : class
    {
        #region Properties
        private ApplicationContext dataContext;
        private readonly DbSet<TEntity> dbSet;

        ////protected IDbFactory DbFactory
        ////{
        ////    get;
        ////    private set;
        ////}

        protected ApplicationContext DbContext
        {
            get {
                return dataContext;// ?? (dataContext = DbFactory.Init());
            }
        }
        #endregion

        protected RepositoryBase(ApplicationContext dbContext)
        {
            dataContext= dbContext;
            dbSet = DbContext.Set<TEntity>();
        }
     
        #region Implementation
        public virtual void Add(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            dbSet.Attach(entity);
            dataContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity)
        {
            dbSet.Remove(entity);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> where)
        {
            IEnumerable<TEntity> objects = dbSet.Where(where).AsEnumerable();
            foreach (TEntity obj in objects)
                dbSet.Remove(obj);
        }      

        public virtual async Task<TEntity> GetByIdAsync(string id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual async Task<int> CountAsync()
        {
            return await dbSet.CountAsync();
        }


        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> where)
        {
            return await dbSet.Where(where).ToListAsync();
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where)
        {
            return await dbSet.Where(where).FirstOrDefaultAsync();
        }    

        #endregion

    }
}
