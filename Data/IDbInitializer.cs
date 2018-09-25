using System.Threading.Tasks;

namespace Data
{
    public interface IDbInitializer
    {
        Task Seed();
    }
}
