using System.Threading.Tasks;

namespace StartUpApi.Data
{
    public interface IDbInitializer
    {
        Task Seed();
    }
}
