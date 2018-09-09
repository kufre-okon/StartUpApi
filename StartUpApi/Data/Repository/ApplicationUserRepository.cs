using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using StartUpApi.Data.DTO;
using Microsoft.EntityFrameworkCore;

using StartUpApi.Utility;
using Pager.Interface;
using Microsoft.AspNetCore.Identity;
using System;
using StartUpApi.Data.Repository.Infrastructure;
using StartUpApi.Data.Models;
using StartUpApi.Data.Models.Extensions;
using StartUpApi.Data.Repository.Interface;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;

namespace StartUpApi.Data.Repository
{
    public class ApplicationUserRepository : RepositoryBase<ApplicationUser>, IApplicationUserRepository
    {
        readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserRepository(ApplicationContext context, UserManager<ApplicationUser> userManager) 
            : base(context)
        {
            _userManager = userManager;
        }

        public async Task<IPaginate<UserDto>> SearchUser(int page, int pageSize, string searchTerm, string roleId, string orderBy, bool orderByASC, bool includeInActive)
        {
            bool isEmptySearchTerm = string.IsNullOrWhiteSpace(searchTerm);
            bool isEmptyRoleId = string.IsNullOrWhiteSpace(roleId);
            orderBy = orderBy ?? "Username";

            var users = await GetPaginatedListAsync(
                selector: u => u.ToUserDto(), // we are selecting the userDTO right in the database, this is only possible with EntityExtentions not with IMapper
                predicate: (u =>
                      ((isEmptySearchTerm || (u.FirstName.ToLower().Contains(searchTerm) || u.Surname.ToLower().Contains(searchTerm))) ||
                      (isEmptySearchTerm || u.UserName.ToLower().Contains(searchTerm))) &&
                      (isEmptyRoleId || u.USERROLES.Any(r => r.RoleId == roleId)) &&
                      (includeInActive || u.IsActive) &&
                      (u.UserName.ToLower() != Constants.SUPER_ADMIN_USERNAME.ToLower())), // exclude superadmin user
                 orderBy: u => (u.OrderBy(orderBy + " " + (orderByASC ? "ASC" : "DESC"))),
                 includes: new List<Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, object>>> {
                     u => u.Include(us => us.USERROLES).ThenInclude(ur => ur.ROLE),
                     u => u.Include(us=>us.USERORGANIZATION).ThenInclude(ur=>ur.ORGANIZATIONUNIT)
                 },
                 index: page,
                 size: pageSize);

            return users;
        }
       

        public async Task CreateUser(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception(string.Join("\n", result.Errors.Select(e => e.Description)));
        }

        public async Task<bool> IsUserLogout(ApplicationUser user)
        {
            return await _userManager.IsLockedOutAsync(user);
        }
        public async Task<ApplicationUser> FindUserByUsername(string username)
        {
            var user = await SingleAsync(
                predicate: u => u.UserName.ToLower() == username.ToLower(),
                orderBy: null,
                includes: new List<Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, object>>>{
                    u => u.Include(t => t.USERROLES).ThenInclude(ur => ur.ROLE),
                    u => u.Include(us=>us.USERORGANIZATION).ThenInclude(ur=>ur.ORGANIZATIONUNIT)
                });
            return user;
        }

        public void UpdateUser(ApplicationUser user)
        {
            Update(user);
        }

    }

    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
 
        Task<IPaginate<UserDto>> SearchUser(int page, int pageSize, string searchTerm, string roleId, string orderBy, bool orderByASC, bool includeInActive);
        Task<ApplicationUser> FindUserByUsername(string username);

        Task<bool> IsUserLogout(ApplicationUser user);
        Task CreateUser(ApplicationUser user, string password);       
    }
}