using Microsoft.EntityFrameworkCore.Storage;
using StartUpApi.Models;
using StartUpApi.Repository.Interface;
using System;
using System.Threading.Tasks;

namespace StartUpApi.Repository.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationContext dbContext;

        private IDbContextTransaction transaction;


        public UnitOfWork(ApplicationContext context)
        {
            this.dbContext = context;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public void BeginTransaction()
        {
            transaction = dbContext.Database.BeginTransaction();
        }

        public void Rollback()
        {
            if (transaction != null)
                transaction.Rollback();
            transaction = null;
        }


        public void Commit()
        {
            if (transaction != null)
                transaction.Commit();
            transaction = null;
        }

        public IDbContextTransaction GetCurrentTransaction()
        {
            return transaction ?? dbContext.Database.CurrentTransaction;
        }
    }
}
