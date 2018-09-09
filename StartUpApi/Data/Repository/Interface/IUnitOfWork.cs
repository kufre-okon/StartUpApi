using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace StartUpApi.Data.Repository.Interface
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

       
        int SaveChanges();
       
        Task<int> SaveChangesAsync();
        
        void BeginTransaction();

        
        void Commit();
       
        void Rollback();

       
        IDbContextTransaction GetCurrentTransaction();
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext dbContext { get; }
    }
}
