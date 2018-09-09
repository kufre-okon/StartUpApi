using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StartUpApi.Data.Models;
using StartUpApi.Data.DTO;
using StartUpApi.Services.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StartUpApi.Controllers
{
    /// <summary>
    /// Manages Role related tasks
    /// </summary>
    [Authorize]
    [Route("api/Role")]
    [Produces("application/json")]
    public class RoleController : BaseController
    {
        readonly IApplicationRoleService _roleService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleService"></param>
        public RoleController(IApplicationRoleService roleService) : base()
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Get light weight list of roles suitable for dropdown list
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("listLightWeight")]
        public async Task<ResponseBase<List<RoleListDto>>> listLightWeight()
        {
            return await ExecuteRequestAsync(async () =>
            {
                var list = await _roleService.GetListLightWeight();
                return list;
            });
        }
    }
}
