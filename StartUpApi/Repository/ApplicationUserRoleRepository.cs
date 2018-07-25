using StartUpApi.Models;
using StartUpApi.Repository.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StartUpApi.Repository.Interface;
using StartUpApi.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace StartUpApi.Repository
{
    public class ApplicationUserRoleRepository : RepositoryBase<ApplicationUserRole>, IApplicationUserRoleRepository
    {
        public ApplicationUserRoleRepository(ApplicationContext context) : base(context)
        {
        }

        private List<PermissionViewModel> GetUserPermissions(List<ApplicationUserRole> roles)
        {
            var permissions = new List<PermissionViewModel>();
            roles.ForEach(r =>
            {
                var perms = permissions.Select(e => e.permissionID);
                permissions.AddRange(r.ROLE.ROLEPERMISSIONS.Where(e => !perms.Contains(e.permissionId)).Select(e => new PermissionViewModel()
                {
                    displayname = e.PERMISSION.DisplayName,
                    name = e.PERMISSION.Name,
                    level = e.PERMISSION.Level,
                    parentid = e.PERMISSION.ParentId,
                    // ParentName = e.PERMISSION.PARENTPERMISSION.name,
                    permissionID = e.permissionId
                }).ToList());
            });
            return permissions;
        }

        public async Task<List<ApplicationUserRole>> GetUserRoles(string userId)
        {
            return await DbContext.ApplicationUserRoles.Where(e => e.UserId == userId).ToListAsync();
        }

        public async Task<List<PermissionViewModel>> GetUserPermissions(string userId)
        {
            var userRoles = await GetUserRoles(userId);
            var perms = GetUserPermissions(userRoles);

            return perms;
        }



        public async Task<bool> AssignDefaultRole(string userId)
        {
            var dr = DbContext.Roles.FirstOrDefault(e => e.Default == true);
            if (dr != null)
            {
                var user = await DbContext.Users.FirstOrDefaultAsync(e => e.Id == userId);
                if (user != null)
                {
                    user.Roles.Clear();
                    user.Roles.Add(new ApplicationUserRole()
                    {
                        RoleId = dr.Id,
                        UserId = user.Id
                    });
                }
            }
            return await DbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AssignRoles(IEnumerable<string> roleIds, string userId)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(e => e.Id == userId);
            if (user != null)
            {
                user.Roles.Clear();
                foreach (var r in roleIds)
                {
                    user.Roles.Add(new ApplicationUserRole() { RoleId = r, UserId = user.Id });
                }
            }
            return await DbContext.SaveChangesAsync() > 0;
        }
    }

    public interface IApplicationUserRoleRepository
    {
        Task<List<PermissionViewModel>> GetUserPermissions(string userId);
        Task<List<ApplicationUserRole>> GetUserRoles(string userId);
        Task<bool> AssignDefaultRole(string userId);
        Task<bool> AssignRoles(IEnumerable<string> roleIds, string userId);
    }
}