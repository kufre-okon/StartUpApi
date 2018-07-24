using StartUpApi.Models;
using System;

namespace StartUpApi.Repository.Interface
{
    public interface IDbFactory : IDisposable
    {
        ApplicationContext Init();

        void SetContext(ApplicationContext dbContext);       
    }
}
