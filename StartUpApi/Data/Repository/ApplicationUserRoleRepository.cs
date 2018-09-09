using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StartUpApi.Data.DTO;
using Microsoft.EntityFrameworkCore;
using StartUpApi.Data.Repository.Infrastructure;
using StartUpApi.Data.Models;
using StartUpApi.Data.Repository.Interface;
using System;
using Microsoft.EntityFrameworkCore.Query;

namespace StartUpApi.Data.Repository
{
    public class ApplicationUserRoleRepository : RepositoryBase<ApplicationUserRole>, IApplicationUserRoleRepository
    {
        public ApplicationUserRoleRepository(ApplicationContext context)
            : base(context)
        {
        }

        public async Task<List<ApplicationUserRole>> GetUserRoles(string userId)
        {
            var result = await GetListAsync(
                    predicate: e => e.UserId == userId,
                    orderBy: null,
                    includes: new List<Func<IQueryable<ApplicationUserRole>, IIncludableQueryable<ApplicationUserRole, object>>>{
                        u => u.Include(ur => ur.ROLE).ThenInclude(r => r.ROLEPERMISSIONS).ThenInclude(r => r.PERMISSION)
                    }
                );
            return result.ToList();
        }

        public async Task<List<PermissionDto>> GetUserPermissions(string userId)
        {
            var userRoles = await GetUserRoles(userId);
            var perms = buildUserPermissions(userRoles);

            return perms;
        }

        public async Task AddUserToRoles(IEnumerable<string> roleIds, string userId)
        {
            var userRoles = await GetListAsync(predicate: r => r.UserId == userId);
            if (userRoles.Count() > 0)
                Delete(userRoles);
            var newuserRoles = new List<ApplicationUserRole>();
            foreach (var r in roleIds)
            {
                newuserRoles.Add(new ApplicationUserRole() { RoleId = r, UserId = userId });
            }
            Add(newuserRoles);
        }

        #region private
        private List<PermissionDto> buildUserPermissions(List<ApplicationUserRole> roles)
        {
            var permissions = new List<PermissionDto>();
            roles.ForEach(r =>
            {
                var perms = permissions.Select(e => e.permissionID);
                permissions.AddRange(r.ROLE.ROLEPERMISSIONS.Where(e => !perms.Contains(e.permissionId)).Select(e => new PermissionDto()
                {
                    displayName = e.PERMISSION.DisplayName,
                    name = e.PERMISSION.Name,
                    level = e.PERMISSION.Level,
                    parentId = e.PERMISSION.ParentId,
                    // ParentName = e.PERMISSION.PARENTPERMISSION.name,
                    permissionID = e.permissionId
                }).ToList());
            });
            return permissions;
        }

        #endregion
    }

    public interface IApplicationUserRoleRepository:IRepository<ApplicationUserRole>
    {
        Task<List<PermissionDto>> GetUserPermissions(string userId);
        Task<List<ApplicationUserRole>> GetUserRoles(string userId);
        Task AddUserToRoles(IEnumerable<string> roleIds, string userId);
    }
}