using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IAuthTokenService
    {
        ApplicationClient FindClient(string clientId);
        Task<bool> AddRefreshToken(RefreshToken token);
        Task<bool> RemoveRefreshToken(string refreshTokenId);
        Task<bool> RemoveRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshToken(string clientId, string username, string refreshToken);
        RefreshToken FindRefreshToken(string refreshTokenId);
        List<RefreshToken> GetAllRefreshTokens();
    }
}
