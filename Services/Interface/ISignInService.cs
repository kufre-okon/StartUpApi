using Data.DTO;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ISignInService
    {
        Task<LoginResponseData> Login(LoginRequestData data, string ip);
        Task<RefreshTokenResponseData> RefreshToken(RefreshTokenRequestData model, string ip);
    }
}
