using StartUpApi.Services.Interface;
using System;
using System.Threading.Tasks;
using StartUpApi.Data.DTO;
using StartUpApi.Utility;
using StartUpApi.Data.Models.Enums;
using StartUpApi.Data.Models;
using Microsoft.AspNetCore.Http.Features;
using StartUpApi.Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using StartUpApi.Data.Models.Extensions;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using StartUpApi.Data.DbScript;
using StartUpApi.Exceptions;

namespace StartUpApi.Services
{
    public class SignInService : ISignInService
    {
        private IPasswordHasher<ApplicationUser> _hasher;
        private IConfiguration _appConfig;
        private IApplicationUserRepository _userRepo;
        private IAuthTokenService _authTokenServ;
        private IDbPatchManager _dbPatchMgr;

        public SignInService(IApplicationUserRepository userRepo,  IPasswordHasher<ApplicationUser> hasher,
          IAuthTokenService authTokenServ, IDbPatchManager dbPatchMgr, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _hasher = hasher;
            _appConfig = configuration;
            _authTokenServ = authTokenServ;
            _dbPatchMgr = dbPatchMgr;
        }

        public async Task<LoginResponseData> Login(LoginRequestData model, string ip)
        {
            if (_dbPatchMgr.GetStatus())
                throw new ApplicationException("Database syncronization in progress, please try to login in few minutes...");
            _dbPatchMgr.Sync();

            var refreshTokenLifeTime = ValidateClientAuthentication(model.ClientId, model.ClientSecret);
            var user = await _userRepo.FindUserByUsername(model.Username);
            if (user == null)
                throw new ApplicationException("Invalid username or password");
            if (await _userRepo.IsUserLogout(user))
                throw new ApplicationException("User account is been logout");
            if (!user.IsActive)
                throw new ApplicationException("User account inactive");
            if (_hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
                throw new ApplicationException("Invalid username or password");
            var jwtSecurityToken = TokenUtility.GenerateJwtSecurityToken(_appConfig.GetSection("AppConfiguration"), TokenUtility.GenerateClaims(user.UserName, user.Id));
            var userVm = UserExtension.BuildUserViewModel(user);
            var refreshToken = TokenUtility.GenerateRefreshToken();
            await SaveRefreshToken(model.ClientId, model.Username, refreshToken, refreshTokenLifeTime, ip);
            user.LastLoginDate = DateTime.UtcNow;
            _userRepo.Update(user);
            var response = new LoginResponseData()
            {
                UserId = userVm.UserID,
                Username = userVm.Username,
                FullName = userVm.FullName,
                ProfilePictureUrl = userVm.ProfilePictureUrl,
                Roles = userVm.Roles,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                TokenExpires = jwtSecurityToken.ValidTo,
                RefreshToken = refreshToken,
                Email = userVm.Email
            };
            return response;
        }

        public async Task<RefreshTokenResponseData> RefreshToken(RefreshTokenRequestData model, string ip)
        {
            var refreshTokenLifeTime = ValidateClientAuthentication(model.ClientId, model.ClientSecret);
            var principal = TokenUtility.GetPrincipalFromExpiredToken(_appConfig.GetSection("AppConfiguration"), model.Token);
            var username = principal.Identity.Name;
            var savedRefreshToken = await _authTokenServ.GetRefreshToken(model.ClientId, username, model.RefreshToken);
            if (savedRefreshToken == null)
                throw new ApplicationException("Invalid refresh token");

            var newJwtToken = TokenUtility.GenerateJwtSecurityToken(_appConfig.GetSection("AppConfiguration"), principal.Claims);
            var newRefreshToken = TokenUtility.GenerateRefreshToken();
            await _authTokenServ.RemoveRefreshToken(savedRefreshToken);
            await SaveRefreshToken(model.ClientId, username, newRefreshToken, refreshTokenLifeTime,  ip);

            return new RefreshTokenResponseData()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(newJwtToken),
                TokenExpires = newJwtToken.ValidTo,
                RefreshToken = newRefreshToken
            };
        }       

        /// <summary>
        ///  Validate and returns 'RefreshTokenLifeTime' attached to the client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret">required only for native client application</param>
        /// <param name="refreshTokenLifeTime"></param>
        /// <returns>RefreshTokenLifeTime</returns>
        private int ValidateClientAuthentication(string clientId, string clientSecret = null)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                throw new Exception("ClientId should be sent");
            var client = _authTokenServ.FindClient(clientId);
            if (client == null)
                throw new Exception(string.Format("Client '{0}' is not registered in the system.", clientId));

            if (client.ApplicationType == ApplicationTypes.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                    throw new Exception("Client secret should be sent for native applications.");
                else
                {
                    var hashedClientSecret = Helper.GetHash(clientSecret);
                    if (client.Secret != hashedClientSecret)
                        throw new Exception("Client secret is invalid.");
                }
            }
            if (!client.Active)
                throw new Exception("Client is inactive.");

            return client.RefreshTokenLifeTime;
        }


        private async Task SaveRefreshToken(string clientid, string username, string newRefreshToken, int refreshTokenLifeTime, string ip)
        {
            var refreshTokenId = Guid.NewGuid().ToString("n");

            var token = new RefreshToken()
            {
                Id = Helper.GetHash(refreshTokenId),
                ClientId = clientid,
                Subject = username,
                IssuedUtc = DateTime.UtcNow, // just set, we are not using it
                LoginSource = ip,
                ExpiresUtc = DateTime.UtcNow.AddHours(refreshTokenLifeTime), // just set, we are not using it
                ProtectedTicket = Helper.GetHash(newRefreshToken)
            };
            var result = await _authTokenServ.AddRefreshToken(token);
        }
    }
}
