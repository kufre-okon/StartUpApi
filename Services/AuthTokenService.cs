using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Repository.Interface;
using Data.Models;
using Services.Interface;
using General.Utilities;

namespace Services
{
    public class AuthTokenService :  IAuthTokenService
    {
        readonly IRepository<RefreshToken> refreshTokenRepo;
        readonly IRepository<ApplicationClient> appClientRepo;
        readonly IUnitOfWork unitOfWork;

        public AuthTokenService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
            refreshTokenRepo = unitOfWork.GetRepository<RefreshToken>();
            appClientRepo = unitOfWork.GetRepository<ApplicationClient>();
            
        }

        public ApplicationClient FindClient(string clientId)
        {
            var client = appClientRepo.GetById(clientId);
            return client;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var existingToken = await refreshTokenRepo.SingleAsync(r => r.Subject == token.Subject && r.ClientId == token.ClientId);
            if (existingToken != null)
            {
                refreshTokenRepo.Delete(existingToken);
            }
            refreshTokenRepo.Add(token);
            return await unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = refreshTokenRepo.GetById(refreshTokenId);
            if (refreshToken != null)
                return await RemoveRefreshToken(refreshToken);
            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            refreshTokenRepo.Delete(refreshToken);
            return await unitOfWork.SaveChangesAsync() > 0;
        }

        public RefreshToken FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = refreshTokenRepo.GetById(refreshTokenId);
            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return refreshTokenRepo.GetList().ToList();
        }

        public async Task<RefreshToken> GetRefreshToken(string clientId, string username, string refreshToken)
        {
            var hashedRefreshToken = Helper.GetHash(refreshToken);
            var _refreshToken = await refreshTokenRepo.SingleAsync(r => r.ClientId == clientId && r.Subject == username && r.ProtectedTicket == hashedRefreshToken);
            return _refreshToken;
        }
    }


    
}