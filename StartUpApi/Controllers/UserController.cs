using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using StartUpApi.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Pager.Interface;
using System.Security.Claims;
using Data.Models;
using Data.DTO;
using General.Exceptions;
using Data.Models.Enums;
using General;
using Services.Interface;
using System.Linq;

namespace StartUpApi.Controllers
{
    /// <summary>
    /// Manages User related tasks
    /// </summary>
    [Authorize]
    [Route("api/User")]
    [Produces("application/json")]
    public class UserController : BaseController
    {
        readonly IUserService _userService;

        /// <summary>
        /// User controller constructure
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="imgService"></param>
        public UserController(IUserService userService) : base()
        {
            _userService = userService;
        }

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ResponseBase<UserDto>> Get(string Id)
        {
            return await ExecuteRequestAsync(async () =>
            {
                var user = await _userService.GetUser(Id);
                user.ProfilePictureUrl= $"{Request.Scheme}://{Request.Host.Value}{user.ProfilePictureUrl}";
                return user;
            });
        }

        /// <summary>
        /// Create new User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseBase<UserDto>> Post([FromBody]UserDto model)
        {
            return await ExecuteRequestAsync(async () =>
               {
                   if (!ModelState.IsValid)
                       throw new ApplicationException(Helper.GetModelStateErrors(ModelState));
                   var result = await _userService.CreateUser(model);
                   if (model.SendActivationEmail)
                   {
                       // codes to send email
                   }
                   return result;
               });
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="model"></param>
        [HttpPut]
        public ResponseBase Put([FromBody]UserDto model)
        {
            return ExecuteRequest(() =>
           {
               if (!ModelState.IsValid)
                   throw new ApplicationException(Helper.GetModelStateErrors(ModelState));
               _userService.UpdateUser(model);
               if (model.SendActivationEmail)
               {
                   // codes to send email
               }
           });
        }

        /// <summary>
        /// Search users
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchTerm"></param>
        /// <param name="roleId"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderMode"></param>
        /// <param name="includeInActive"></param>
        /// <returns></returns>
        [HttpGet, Route("search")]
        public async Task<ResponseBase<IPaginate<UserDto>>> SearchUser([FromQuery] int page = 1, [FromQuery]int pageSize = 10, [FromQuery]string searchTerm = "",
            [FromQuery]string roleId = "", [FromQuery] string orderBy = "Username", [FromQuery]OrderByEnum orderMode = OrderByEnum.ASC, [FromQuery]bool includeInActive = true)
        {
            return await ExecuteRequestAsync(async () =>
           {
               var result = await _userService.SearchUser(page, pageSize, searchTerm, roleId, orderBy, (orderMode == OrderByEnum.ASC), includeInActive);
               result.Items.ToList().ForEach(e => e.ProfilePictureUrl = $"{Request.Scheme}://{Request.Host.Value}{e.ProfilePictureUrl}");

               return result;
           });
        }

        /// <summary>
        /// Change current login user password
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, Route("ChangePassword")]
        public async Task<ResponseBase> ChangePassword([FromBody]PasswordChangeRequest data)
        {
            return await ExecuteRequestAsync(async () =>
            {
                var userId = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await _userService.ChangePassword(data, userId);
            });
        }

        /// <summary>
        /// Update user roles
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}/updateRole")]
        public async Task<ResponseBase> PutUserRoles(string Id, [FromBody]List<string> roles)
        {
            return await ExecuteRequestAsync(async () =>
            {
                await _userService.UpdateUserRole(Id, roles);
            });
        }

        /// <summary>
        /// Reset User special permissions and return role based permissions
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}/resetSpecialPermissions")]
        public async Task<ResponseBase> ResetPermissions(string Id)
        {
            return await ExecuteRequestAsync(async () =>
            {
                await _userService.ResetSpecialPerimssions(Id);

            });
        }

        /// <summary>
        /// Change user status to "Active" or "InActive"
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost, Route("{id}/changeStatus")]
        public ResponseBase ChangeUserAccountStatus(string Id, [FromBody]bool status)
        {
            return ExecuteRequest(() =>
           {
               _userService.ToggleUserAccount(Id, status);
           });
        }

        /// <summary>
        /// Get user permissions
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}/permissions")]
        public async Task<ResponseBase<List<PermissionDto>>> GetUserPermissions(string Id)
        {
            return await ExecuteRequestAsync(() =>
           {
               var username = HttpContext.User?.FindFirst(ClaimTypes.Name)?.Value;

               var perms = _userService.GetUserPermissions(Id, username.Equals(Constants.SUPER_ADMIN_USERNAME, StringComparison.CurrentCultureIgnoreCase));
               return perms;
           });
        }

        /// <summary>
        /// Update user special permissions
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}/UpdateSpecialPermissions")]
        public async Task<ResponseBase> UpdateSpecialPermissions(string Id, [FromBody] List<UserSpecialPermissionData> data)
        {
            return await ExecuteRequestAsync(async () =>
            {
                await _userService.UpdateUserSpecialPermissions(Id, data);
            });
        }

        /// <summary>
        /// Reset user password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("ResetPassword")]
        public async Task<ResponseBase> ResetPassword([FromBody]PasswordResetRequest model)
        {
            return await ExecuteRequestAsync(async () =>
            {
                await _userService.ResetPassword(model);
            });
        }

        /// <summary>
        /// Get total users
        /// </summary>        
        /// <returns></returns>
        [HttpGet, Route("UserCount")]
        public async Task<ResponseBase<UserCountDTO>> UserCount()
        {
            return await ExecuteRequestAsync(async () =>
            {
                return await _userService.CountUsers();
            });
        }
    }
}