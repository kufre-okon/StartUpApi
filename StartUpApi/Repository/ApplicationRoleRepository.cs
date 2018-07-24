using CuzaEnterprise.API.Models;
using CuzaEnterprise.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CuzaEnterprise.API.Repositories
{
    public interface IApplicationRoleRepository
    {
        Task<ApplicationRole> AddRole(ApplicationRole role);
        Task<ApplicationRole> GetRole(string Id);
        Task<bool> UpdateRole(ApplicationRole menu);
        Task<List<RoleViewModel>> GetAllRoles();
        Task<List<RoleViewModel>> GetRoles(int tenantId);
        Task<List<RoleViewModel>> GetRoles(int tenantId,int permissionId);
        Task UpdateRolePermission(string roleId, List<int> permissionIds);
        Task UpdateRoleUsers(string roleId, List<string> userIds);
        Task<List<PermissionViewModel>> GetRolePermissions(string roleId);
        Task<List<UserViewModel>> GetRoleUsers(string id);
        Task Delete(string id);
    }

    public class ApplicationRoleRepository : RepositoryBase<ApplicationUser>, IApplicationRoleRepository, IDisposable
    {

        public ApplicationRoleRepository(ApplicationDbContext appDbContext)
        {
            _context = appDbContext ?? ApplicationDbContext.Create();
        }

        public ApplicationRoleRepository()
        {
            _context = ApplicationDbContext.Create();
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public async Task<ApplicationRole> AddRole(ApplicationRole role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            addOrUpdatePermissions(role.Id, role.ROLEPERMISSIONS.ToList());
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<List<RoleViewModel>> GetAllRoles()
        {
            var roles = await _context.Roles
                .Select(e => new RoleViewModel()
                {
                    TenantId = e.TenantId,
                    Default = e.Default,
                    Description = e.Description,
                    Id = e.Id,
                    IsStatic = e.IsStatic,
                    Name = e.Name,
                     DateCreated=e.DateCreated,
                    TenantName = e.TENANT == null ? "" : e.TENANT.BusinessName,
                    AssignedPermissions = e.ROLEPERMISSIONS.Select(p => p.permissionId).ToList()
                }).ToListAsync();
            return roles;
        }

        public async Task<List<RoleViewModel>> GetRoles(int tenantId)
        {

            var roles = await _context.Roles
                .Where(e => e.TenantId == tenantId)
                .Select(e => new RoleViewModel()
                {
                    TenantId = e.TenantId,
                    Default = e.Default,
                    Description = e.Description,
                    Id = e.Id,
                    DateCreated = e.DateCreated,
                    IsStatic = e.IsStatic,
                    Name = e.Name,
                    AssignedPermissions = e.ROLEPERMISSIONS.Select(p => p.permissionId).ToList()
                }).ToListAsync();
            return roles;
        }

        public async Task<List<RoleViewModel>> GetRoles(int tenantId, int permissionId)
        {

            var roles = await _context.RolePermissions
                .Where(e => e.permissionId == permissionId && e.ROLE.TenantId== tenantId)
                .Select(e => new RoleViewModel()
                {
                    TenantId = e.ROLE.TenantId,
                    Default = e.ROLE.Default,
                    Description = e.ROLE.Description,
                    Id = e.ROLE.Id,
                    DateCreated = e.ROLE.DateCreated,
                    IsStatic = e.ROLE.IsStatic,
                    Name = e.ROLE.Name,
                    AssignedPermissions = e.ROLE.ROLEPERMISSIONS.Select(p => p.permissionId).ToList()
                }).ToListAsync();
            return roles;
        }

        public async Task<bool> UpdateRole(ApplicationRole role)
        {
            var _role = _context.Roles.Find(role.Id);
            _role.IsStatic = role.IsStatic;
            _role.Name = role.Name;
            _role.TenantId = role.TenantId;
            _role.Default = role.Default;
            _role.DateCreated = role.DateCreated;
            _role.Description = role.Description;
            addOrUpdatePermissions(_role.Id, role.ROLEPERMISSIONS.ToList());
            return await _context.SaveChangesAsync() > 0;
        }

        private void addOrUpdatePermissions(string roleId, List<ApplicationRolePermission> permissions)
        {
            addOrUpdatePermissions(roleId, permissions.Select(e => e.permissionId).ToList());
        }

        private void addOrUpdatePermissions(string roleId, List<int> permissionIds)
        {
            var _rolePerms = _context.RolePermissions.Where(r => r.roleId == roleId).ToList();
            _rolePerms.ForEach(rolePerm =>
            {
                _context.RolePermissions.Remove(rolePerm);
            });
            var rolePerms = permissionIds.ConvertAll(e => new ApplicationRolePermission()
            {
                roleId = roleId,
                permissionId = e
            });
            _context.RolePermissions.AddRange(rolePerms);
        }

        public async Task<ApplicationRole> GetRole(string Id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(e => e.Id == Id);
            return role;
        }

        public async Task UpdateRolePermission(string roleId, List<int> permissionIds)
        {
            addOrUpdatePermissions(roleId, permissionIds);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRoleUsers(string roleId, List<string> userIds)
        {
            var roleUser = _context.UserRoles.FirstOrDefault(r => r.RoleId == roleId);
            if (roleUser != null)
            {
                _context.UserRoles.Remove(roleUser);
            }
            var roleUsers = userIds.ConvertAll(e => new ApplicationUserRole()
            {
                RoleId = roleId,
                UserId = e
            });
            _context.UserRoles.AddRange(roleUsers);

            await _context.SaveChangesAsync();
        }

        public async Task<List<PermissionViewModel>> GetRolePermissions(string roleId)
        {
            var perms = await _context.RolePermissions
                .Where(e => e.roleId == roleId)
                .Select(e => new PermissionViewModel()
                {
                    displayname = e.PERMISSION.displayname,
                    level = e.PERMISSION.level,
                    name = e.PERMISSION.name,
                    parentid = e.PERMISSION.parentid,
                    permissionID = e.PERMISSION.permissionID
                }).ToListAsync();
            return perms;
        }

        public async Task<List<UserViewModel>> GetRoleUsers(string id)
        {
            var perms = await _context.UserRoles
               .Where(e => e.RoleId == id)
               .Select(e => new UserViewModel()
               {
                   UserID = e.UserId,
                   Username = e.USER.UserName,
                   FirstName = e.USER.FirstName,
                   Email = e.USER.Email,
                   IsActive = e.USER.IsActive,
                   LockoutEnabled = e.USER.LockoutEnabled,
                   LastName = e.USER.Surname,
                   DateCreated = e.USER.DateCreated,
                   IsTemporaryPassword = e.USER.IsTemporaryPassword,
                   LastLoginDate = e.USER.LastLoginDate,
                   TenantId = e.USER.TenantId,
                   ProfilePictureUrl = "",
                   FullName = e.USER.FirstName + " " + e.USER.Surname,
                   IsTenantOwner = e.USER.IsTenantOwner,
                    IsEmailVerified=e.USER.EmailConfirmed
               }).ToListAsync();
            return perms;
        }

        private void deleteRolePermission(string id)
        {
            var role = _context.RolePermissions.Where(e => e.roleId == id).ToList();
            role.ForEach(e =>
            {
                _context.RolePermissions.Remove(e);
            });
        }

        private void deleteUserRole(string id)
        {
            var role = _context.UserRoles.Where(e => e.RoleId == id).ToList();
            role.ForEach(e =>
            {
                _context.UserRoles.Remove(e);
            });
        }

        private List<string> getUsersInRole(string roleId)
        {
            return _context.UserRoles
                .Where(e => e.RoleId == roleId)
                .Select(e => e.UserId)
                .ToList();
        }

        public async Task Delete(string id)
        {
            var _trans = _context.Database.BeginTransaction();
            try
            {
                var role = _context.Roles.FirstOrDefault(e => e.Id == id);
                if (role.IsStatic)
                    throw new Exception("Static Role cannot be deleted");
                // get all userId belonging to this role
                var userIds = getUsersInRole(id);
                // delete associated role-permissions
                deleteRolePermission(id);
                if (userIds.Count < 1)
                    _context.Roles.Remove(role);
                else
                {
                    // check if there is any default role
                    var dr = _context.Roles.FirstOrDefault(e => e.Default == true);
                    if (dr == null)
                        throw new ApplicationException("No Default Role");
                    // delete associated user-role                  
                    deleteUserRole(id);
                    // check if users already belong to the default group then skip
                    var usersInGroup = getUsersInRole(dr.Id);
                    userIds = userIds.SkipWhile(e => usersInGroup.Contains(e)).ToList();
                    // add the users above to the default group
                    await UpdateRoleUsers(dr.Id, userIds);
                    // delete role
                    _context.Roles.Remove(role);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _trans.Rollback();
                throw;
            }
            _trans.Commit();
        }
    }
}