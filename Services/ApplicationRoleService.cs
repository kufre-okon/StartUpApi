using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Data.DTO;
using Data.Models;
using Data.Models.Enums;
using Data.Models.Extensions;
using Data.Repository.Interface;
using Data.Repository;
using General.Exceptions;
using Services.Base;
using Services.Interface;

namespace Services
{
    public class ApplicationRoleService : ServiceBase, IApplicationRoleService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IRepository<ApplicationRole> _roleRepo;
        public IApplicationUserRoleRepository _userRoleRepo;

        public ApplicationRoleService(IApplicationUserRoleRepository userRoleRepo, IUnitOfWork unitOfWork, IAuditTrailManager auditTrailManager, IHttpContextAccessor contextAccessor) : base(auditTrailManager, contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _roleRepo = _unitOfWork.GetRepository<ApplicationRole>();
            _userRoleRepo = userRoleRepo;
        }

        public void AddRole(RoleDto role)
        {
            ExecuteVoid(true, AuditTrailOperations.CreateRole, _unitOfWork, () =>
           {
               _roleRepo.Add(role.ToApplicationRole());
               addOrUpdatePermissions(role.Id, role.AssignedPermissions.ToList());
               return null;
           });
        }

        public async Task<List<RoleDto>> GetList()
        {
            var roles = await _roleRepo.GetListAsync(selector: role => role.ToRoleDto(),
                includes: new List<Func<IQueryable<ApplicationRole>, IIncludableQueryable<ApplicationRole, object>>> { role => role.Include(r => r.ROLEPERMISSIONS) });
            return roles.ToList();
        }
        public async Task<List<RoleListDto>> GetListLightWeight()
        {
            var roles = await _roleRepo.GetListAsync(selector: role => role.ToRoleListDto(),
                includes: new List<Func<IQueryable<ApplicationRole>, IIncludableQueryable<ApplicationRole, object>>> { role => role.Include(r => r.ROLEPERMISSIONS) });
            return roles.ToList();
        }

        public async Task<List<RoleDto>> GetByPermission(int permissionId)
        {

            var roles = await _roleRepo.GetListAsync(
                    predicate: role => role.ROLEPERMISSIONS.Any(p => p.permissionId == permissionId),
                    selector: role => role.ToRoleDto(),
                    includes: new List<Func<IQueryable<ApplicationRole>, IIncludableQueryable<ApplicationRole, object>>> { role => role.Include(r => r.ROLEPERMISSIONS) }
                );
            return roles.ToList();
        }

        public async Task UpdateRole(ApplicationRole role)
        {
            await ExecuteVoidAsync(true, AuditTrailOperations.UpdateRole, _unitOfWork, () =>
            {
                _roleRepo.Update(role);

                addOrUpdatePermissions(role.Id, role.ROLEPERMISSIONS.ToList());
                return null;
            });
        }

        public async Task<RoleDto> GetRole(string Id)
        {
            var role = await _roleRepo.SingleAsync(e => e.Id == Id);
            return role.ToRoleDto();
        }

        public void UpdateRolePermission(string roleId, List<int> permissionIds)
        {
            ExecuteVoid(true, AuditTrailOperations.UpdateRole, _unitOfWork, () =>
           {
               addOrUpdatePermissions(roleId, permissionIds);
               return null;
           }, $"Updated role #{roleId} permissions");
        }

        public async Task UpdateRoleUsers(string roleId, List<string> userIds)
        {
            await ExecuteAsync(true, AuditTrailOperations.UpdateRole, _unitOfWork, async () =>
             {
                 var roleUsers = _userRoleRepo.GetList(predicate: r => r.RoleId == roleId);
                 _userRoleRepo.Delete(roleUsers);

                 var newRoleUsers = userIds.Select(e => new ApplicationUserRole()
                 {
                     RoleId = roleId,
                     UserId = e
                 });
                 await _userRoleRepo.AddAsync(newRoleUsers);
                 return string.Empty;
             }, $"Add/Remove users from role #{roleId}");
        }

        public async Task<List<PermissionDto>> GetRolePermissions(string roleId)
        {
            var role = await _roleRepo.SingleAsync(
                predicate: r => r.Id == roleId,
                includes: new List<Func<IQueryable<ApplicationRole>, IIncludableQueryable<ApplicationRole, object>>>{
                    r => r.Include(t => t.ROLEPERMISSIONS).ThenInclude(rp => rp.PERMISSION)
                });
            var perms = role.ROLEPERMISSIONS.Select(e => new PermissionDto()
            {
                displayName = e.PERMISSION.DisplayName,
                level = e.PERMISSION.Level,
                name = e.PERMISSION.Name,
                parentId = e.PERMISSION.ParentId,
                permissionID = e.PERMISSION.ID
            }).ToList();
            return perms;
        }

        public async Task<List<UserDto>> GetRoleUsers(string id)
        {
            var users = await _userRoleRepo.GetListAsync(
                predicate: ur => ur.RoleId == id,
               includes: new List<Func<IQueryable<ApplicationUserRole>, IIncludableQueryable<ApplicationUserRole, object>>>{
                   ur => ur.Include(t => t.USER)
               },
               selector: e => new UserDto()
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
                   ProfilePictureUrl = e.USER.ProfilePictureUrl,
                   FullName = e.USER.FirstName + " " + e.USER.Surname,
                   IsEmailVerified = e.USER.EmailConfirmed
               });
            return users.ToList();
        }

        public async Task Delete(string id)
        {
            await ExecuteAsync(true, AuditTrailOperations.DeleteRole, _unitOfWork, async () =>
            {
                var role = _roleRepo.GetById(id);
                if (role.IsStatic)
                    throw new ApplicationException("Static Roles cannot be deleted");
                // get all userId belonging to this role
                var userIds = getUsersInRole(id);
                // delete associated role-permissions
                deleteRolePermission(id);
                if (userIds.Count < 1)
                    _roleRepo.Delete(role);
                else
                {
                    // check if there is any default role
                    var dr = _roleRepo.Single(e => e.Default == true);
                    if (dr == null)
                        throw new ApplicationException("No Default Role");
                    // delete associated user-role                  
                    _userRoleRepo.Delete(e => e.RoleId == id);
                    // check if users already belong to the default group then skip
                    var usersInGroup = getUsersInRole(dr.Id);
                    userIds = userIds.SkipWhile(e => usersInGroup.Contains(e)).ToList();
                    // add the users above to the default group
                    await UpdateRoleUsers(dr.Id, userIds);
                    // delete role
                    _roleRepo.Delete(role);
                }
                return string.Empty;
            });
        }


        #region privates 

        private void deleteRolePermission(string id)
        {
            var _rolePermRepo = _unitOfWork.GetRepository<ApplicationRolePermission>();
            _rolePermRepo.Delete(r => r.roleId == id);
        }

        private List<string> getUsersInRole(string roleId)
        {
            var users = _userRoleRepo.GetList(
                    predicate: ur => ur.RoleId == roleId,
                    includes: new List<Func<IQueryable<ApplicationUserRole>, IIncludableQueryable<ApplicationUserRole, object>>>{
                        ur => ur.Include(t => t.USER)
                    },
                    selector: e => e.UserId
                );
            return users.ToList();
        }

        private void addOrUpdatePermissions(string roleId, List<ApplicationRolePermission> permissions)
        {
            addOrUpdatePermissions(roleId, permissions.Select(e => e.permissionId).ToList());
        }

        private void addOrUpdatePermissions(string roleId, List<int> permissionIds)
        {
            var _rolePermRepo = _unitOfWork.GetRepository<ApplicationRolePermission>();
            deleteRolePermission(roleId);

            var rolePerms = permissionIds.Select(e => new ApplicationRolePermission()
            {
                roleId = roleId,
                permissionId = e
            });
            _rolePermRepo.Add(rolePerms);
        }

        #endregion

    }
}