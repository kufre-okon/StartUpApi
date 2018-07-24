using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace StartUpApi.Repository.Interface
{
    public interface IUnitOfWork
    {              
        /// <summary>
        /// Saves the underlying changes to the database
        /// </summary>
        void SaveChanges();
        /// <summary>
        /// Saves the underlying changes to the database asynchronously
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Initiate Database transaction on the current context
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits the current transaction
        /// </summary>
        void Commit();
        /// <summary>
        /// Rollback the current transaction
        /// </summary>
        void Rollback();

        /// <summary>
        /// Get current transaction
        /// </summary>
        /// <returns></returns>
        IDbContextTransaction GetCurrentTransaction();
    }
}
