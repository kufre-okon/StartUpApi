using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Interface;
using Microsoft.AspNetCore.Http.Features;
using System;
using Data.Models;
using Data.DTO;
using General;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StartUpApi.Controllers
{
    /// <summary>
    /// Manage account related tasks
    /// </summary>
    [Route("api/account")]
    [Produces("application/json")]
    public class AccountController : BaseController
    {

        readonly ISignInService _signInService;
        readonly IUserService _userService;

        /// <summary>
        /// Manage account related tasks
        /// </summary>
        /// <param name="signInService"></param>
        /// <param name="userService"></param>
        public AccountController(ISignInService signInService, IUserService userService) : base()
        {
            _signInService = signInService;
            _userService = userService;
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ResponseBase<LoginResponseData>> Login([FromBody]LoginRequestData model)
        {
            return await ExecuteRequestAsync(async () =>
            {
                var ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
                var response = await _signInService.Login(model, ip);
                response.Permissions = await _userService.GetUserPermissions(response.UserId, response.Username.Equals(Constants.SUPER_ADMIN_USERNAME, StringComparison.CurrentCultureIgnoreCase));
                return response;
            });
        }

        /// <summary>
        /// Refreshes login user token
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Refreshed token information</returns>
        [Authorize]
        [HttpPost("RefreshToken")]
        public async Task<ResponseBase<RefreshTokenResponseData>> RefreshToken([FromBody] RefreshTokenRequestData model)
        {
            return await ExecuteRequestAsync(() =>
            {
                var ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
                return _signInService.RefreshToken(model, ip);
            });
        }

        /// <summary>
        /// Checks if the login token is still active. It returns nothing if still active but return 401 response status on token expiration
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("checkToken")]
        public ResponseBase checkToken()
        {
            return ExecuteRequest(() =>
           {
                //  return string.Empty;
            });
        }

        /// <summary>
        /// Check if username exist
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("UsernameExist/{username}")]
        public async Task<ResponseBase<bool>> UsernameExist(string username)
        {
            return await ExecuteRequestAsync(() =>
            {
                return _userService.UserExists(username);
            });
        }

    }
}
