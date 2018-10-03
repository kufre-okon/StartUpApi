using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Pager.Interface;
using Microsoft.EntityFrameworkCore.Query;
using Data.DTO;
using Data.Repository;
using Data.Models;
using Data.Repository.Interface;
using Data.Models.Enums;
using Data.Models.Extensions;
using General.Exceptions;
using Data.Models.Comparer;
using Services.Base;
using Services.Interface;

namespace Services
{
    public class UserService : ServiceBase, IUserService
    {
        readonly IApplicationUserRepository _userRepo;
        readonly IApplicationUserRoleRepository _userRoleRepo;
        readonly IUnitOfWork _unitOfWork;
        readonly UserManager<ApplicationUser> _userManager;

        public UserService(IApplicationUserRepository userRepo, IApplicationUserRoleRepository userRoleRepo, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IAuditTrailManager auditTrailManager, IHttpContextAccessor contextAccessor)
            : base(auditTrailManager, contextAccessor)
        {
            _userRoleRepo = userRoleRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<UserDto> GetUser(string userId)
        {
            var user = _userRepo.Single(predicate: u => u.Id == userId,
                includes: new List<Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, object>>>{
                    u => u.Include(t => t.USERROLES).ThenInclude(ur=>ur.ROLE)
                });
            var uvm = user.ToUserDto();
            uvm.Permissions = await _userRoleRepo.GetUserPermissions(userId);
            return uvm;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersByRole(string roleId)
        {
            var users = await _userRepo.GetListAsync(
                 predicate: u => u.USERROLES.Any(r => r.RoleId == roleId),
                 includes: new List<Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, object>>> { u => u.Include(t => t.USERROLES).ThenInclude(ur => ur.ROLE) });
            return users;
        }

        public void DeleteUser(ApplicationUser user)
        {
            _userRepo.Delete(user);
        }

        public void DeleteUser(string userId)
        {
            _userRepo.Delete(userId);
        }

        public async Task<int> Count()
        {
            return await _userRepo.CountAsync();
        }

        public async Task<List<PermissionDto>> GetUserPermissions(string userId, bool isSuperAdmin = false)
        {
            var _permRepo = _unitOfWork.GetRepository<Permission>();
            var finalList = new List<PermissionDto>();
            if (isSuperAdmin)
            {
                finalList = (await _permRepo.GetListAsync(selector: p => p.ToPermissionDto())).ToList();
            }
            else
            {
                var _userPermRepo = _unitOfWork.GetRepository<UserPermission>();
                var _userRoleRepo = _unitOfWork.GetRepository<ApplicationUserRole>();
                var _userRolePermRepo = _unitOfWork.GetRepository<ApplicationRolePermission>();
                // get the roleIds this user belong to
                var userRoleIDs = _userRoleRepo.GetList(predicate: ur => ur.UserId == userId,
                    includes: new List<Func<IQueryable<ApplicationUserRole>, IIncludableQueryable<ApplicationUserRole, object>>> { ur => ur.Include(t => t.ROLE) }, selector: ur => ur.ROLE.Id);
                // get the permissions assigned to this roles
                var rolePerms = _userRolePermRepo.GetList(predicate: ur => userRoleIDs.Contains(ur.ROLE.Id),
                    includes: new List<Func<IQueryable<ApplicationRolePermission>, IIncludableQueryable<ApplicationRolePermission, object>>> { rp => rp.Include(t => t.PERMISSION) }, selector: rp => rp.PERMISSION).ToList();
                // get user special permissions
                var userPerms = (await _userPermRepo.GetListAsync(predicate: ur => ur.userId == userId,
                    includes: new List<Func<IQueryable<UserPermission>, IIncludableQueryable<UserPermission, object>>> { ur => ur.Include(r => r.PERMISSION) }, selector: ur => ur.PERMISSION)).ToList();
                // let join the two permissions               
                var finalPerms = rolePerms.Union(userPerms, new GenericComparer<Permission>((p1, p2) => p1.ID == p2.ID, p => p.ID.GetHashCode()));

                finalList = finalPerms.Select(p => p.ToPermissionDto()).ToList();
            }
            return finalList;
        }

        public async Task ChangePassword(PasswordChangeRequest data, string userId)
        {
            await ExecuteAsync(false, AuditTrailOperations.ChangePassword, _unitOfWork, async () =>
            {
                var user = _userRepo.GetById(userId);

                var re = await _userManager.ChangePasswordAsync(user, data.CurrentPassword, data.NewPassword);
                if (!re.Succeeded && re.Errors.Count() > 0)
                    throw new ApplicationException(string.Join("\n", re.Errors.Select(err => err.Description)));
                if (user.IsTemporaryPassword)
                {
                    user.IsTemporaryPassword = false;
                    _userRepo.Update(user);
                }
                return string.Empty;
            });
        }

        public async Task UpdateUser(UserDto user)
        {
            await ExecuteAsync(true, AuditTrailOperations.UpdateUser, _unitOfWork, async () =>
            {
                var _user = user.ToApplicationUser();
                var oldUser = _userRepo.Single(e => e.Id == user.UserID);
                _user.PasswordHash = oldUser.PasswordHash;
                _user.ProfilePictureUrl = oldUser.ProfilePictureUrl; // profile picture are set using a different API
                _user.USERROLES.Clear();
               // The default LockoutEnabled property for a User is not the property indicating if a user is currently being locked out or not. It's a property indicating if the user should be subject to lockout or not once the AccessFailedCount reaches the MaxFailedAccessAttemptsBeforeLockout value
               if (user.LockoutEnabled)
                {
                    _user.LockoutEnd = DateTime.UtcNow.AddYears(60); // set date very far in the future to enforce logout
               }
                else
                {
                    _user.LockoutEnd = null;
                }
                _userRepo.Update(_user);
                await updateUserRoles(user.UserID, user.Roles.Select(e => e.Id).ToList());
                return string.Empty;
            });
        }

        public async Task<IPaginate<UserDto>> SearchUser(int page, int pageSize, string searchTerm, string roleId, string orderBy, bool orderByASC, bool includeInActive)
        {
            bool isEmptySearchTerm = string.IsNullOrWhiteSpace(searchTerm);
            bool isEmptyRoleId = string.IsNullOrWhiteSpace(roleId);
            orderBy = orderBy ?? "Username";
            var result = await _userRepo.SearchUser(page, pageSize, searchTerm, roleId, orderBy, orderByASC, includeInActive);

            return result;
        }

        public async Task UpdateUserRole(string userId, List<string> roles)
        {
            await ExecuteAsync(true, AuditTrailOperations.UpdateUserRole, _unitOfWork, async () =>
            {
                await updateUserRoles(userId, roles);
                return string.Empty;
            });
        }

        public void ToggleUserAccount(string userId, bool enable)
        {
            var u = _userRepo.GetById(userId);
            ExecuteVoid(false, enable ? AuditTrailOperations.EnableUserAccount : AuditTrailOperations.DisableUserAccount, _unitOfWork, () =>
           {
               u.IsActive = enable;
               _userRepo.Update(u);
               return null;
           }, $"{(enable ? "Enabled" : "Disabled")} user #{u.UserName}");
        }

        public async Task ResetPassword(PasswordResetRequest model)
        {
            var user = _userRepo.GetById(model.UserId);
            await ExecuteAsync(false, AuditTrailOperations.ResetPassword, _unitOfWork, async () =>
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (result.Succeeded)
                {
                    user.IsTemporaryPassword = true;
                    _userRepo.Update(user);
                }
                else
                {
                    throw new ApplicationException(string.Join("\n", result.Errors));
                }
                return string.Empty;
            }, $"Reset password for #{user.UserName}");
        }

        public async Task ResetSpecialPerimssions(string userId)
        {
            var _userPermRepo = _unitOfWork.GetRepository<UserPermission>();
            _userPermRepo.Delete(e => e.userId == userId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateUserSpecialPermissions(string userId, List<UserSpecialPermissionData> permissions)
        {
            await ExecuteAsync(true, AuditTrailOperations.Others, _unitOfWork, async () =>
            {
                var _userPermRepo = _unitOfWork.GetRepository<UserPermission>();

                var userPerms = _userPermRepo.GetList(predicate: up => up.userId == userId);
                _userPermRepo.Delete(userPerms);

                var newUserPerms = permissions.Where(e => e.included).Select(e => new UserPermission() { userId = userId, permissionId = e.permissionId }).ToList();
                await _userPermRepo.AddAsync(newUserPerms);

                return string.Empty;
            }, "Update user special permissions");
        }

        public async Task<UserCountDTO> CountUsers()
        {
            var users = await _userRepo.GetListAsync();
            var result = new UserCountDTO()
            {
                ActiveUsers = users.Where(e => e.IsActive).Count(),
                InActiveUsers = users.Where(e => !e.IsActive).Count()
            };
            return result;
        }

        public async Task<UserDto> CreateUser(UserDto user)
        {
            return await ExecuteAsync(true, AuditTrailOperations.CreateUser, _unitOfWork, async () =>
          {
              var _roleRepo = _unitOfWork.GetRepository<ApplicationRole>();

              var newUser = user.ToApplicationUser();
              var olduser = await FindByUsername(user.Username);
              if (olduser != null)
                  throw new ApplicationException("Username already taken");

              if (user.Roles.Count < 1)
              {
                  // assign default role
                  var _defaultRole = _roleRepo.Single(e => e.Default);
                  if (_defaultRole != null)
                      newUser.USERROLES.Add(new ApplicationUserRole() { RoleId = _defaultRole.Id, UserId = newUser.Id });
              }
              newUser.EmailConfirmed = !user.SendActivationEmail;
              newUser.SecurityStamp = Guid.NewGuid().ToString("D");
              var result = await _userManager.CreateAsync(newUser, user.Password);
              if (!result.Succeeded)
                  throw new ApplicationException(string.Join("\n", result.Errors.Select(e => e.Description)));
              return newUser.ToUserDto();
          });
        }

        public async Task<ApplicationUser> FindByUsername(string username)
        {
            return await _userRepo.SingleAsync(e => e.UserName.ToLower() == username.ToLower());
        }

        public async Task<bool> UserExists(string username)
        {
            return await FindByUsername(username) != null;
        }

        #region private 

        private async Task updateUserRoles(string userId, List<string> roles)
        {
            _userRoleRepo.Delete(e => e.UserId == userId);
            _unitOfWork.SaveChanges(); // this is necessary because userId above is being tracked
            var _roles = roles.Select(e => new ApplicationUserRole()
            {
                RoleId = e,
                UserId = userId
            });
            await _userRoleRepo.AddAsync(_roles);
        }

        #endregion
    }
}