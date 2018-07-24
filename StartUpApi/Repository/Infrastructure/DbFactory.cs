using System;
using Microsoft.AspNetCore.Server.Kestrel.Internal;
using StartUpApi.Models;
using StartUpApi.Repository.Interface;

namespace StartUpApi.Repository.Infrastructure
{
    //For Local Deployment
    public class DbFactory: IDbFactory
    {
       
        ApplicationContext dbContext = new  ApplicationContext();

        public void Dispose()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }

        public ApplicationContext Init()
        {
            return dbContext;
        }

        /// <summary>
        /// Set custom DbContext useful during unit testing so that the mock CuzaEnterpriseContext can be passed
        /// </summary>
        /// <param name="dbContext"></param>
        public virtual void SetContext(ApplicationContext dbContext)
        {
            this.dbContext = dbContext;
        }

       
    }
}
