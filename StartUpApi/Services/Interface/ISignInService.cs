using StartUpApi.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StartUpApi.Services.Interface
{
    public interface ISignInService
    {
        Task<LoginResponseData> Login(LoginRequestData data, string ip);
        Task<RefreshTokenResponseData> RefreshToken(RefreshTokenRequestData model, string ip);
    }
}
