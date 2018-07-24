using StartUpApi.Models;
using StartUpApi.Repository.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using StartUpApi.ViewModels;
using Microsoft.EntityFrameworkCore;

using StartUpApi.Utility;
using Pager.Interface;
using Pager.Extension;

namespace StartUpApi.Repository
{
    public class ApplicationUserRepository : RepositoryBase<ApplicationUser>, IApplicationUserRepository
    {
        IApplicationUserRoleRepository ApplicationUserRoleRepo;
        public ApplicationUserRepository(ApplicationContext context, IApplicationUserRoleRepository applicationUserRoleRepository) : base(context)
        {
            ApplicationUserRoleRepo = applicationUserRoleRepository;
        }


        public async Task<UserViewModel> GetUser(string userId)
        {
            var user = await GetByIdAsync(userId);           
            var uvm = new UserViewModel()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                IsActive = user.IsActive,
                LockoutEnabled = user.LockoutEnabled,
                LastName = user.Surname,
                FullName = user.FirstName + " " + user.Surname,
                Roles = user.ROLES.Select(e => new IdNamePair<string>() { Id = e.Id, Name = e.Name }).ToList(),
                UserID = user.Id,
                Username = user.UserName,
                DateCreated = user.DateCreated,
                IsTemporaryPassword = user.IsTemporaryPassword,
                LastLoginDate = user.LastLoginDate,
                PhoneNumber = user.PhoneNumber,
                IsEmailVerified = user.EmailConfirmed
            };
            uvm.Permissions = await ApplicationUserRoleRepo.GetUserPermissions(userId);
            return uvm;
        }

        public async Task<List<UserViewModel>> GetUsers()
        {
            var users = await DbContext.Users
                .Select(user => buildUserViewModel(user)).ToListAsync();
            return users;
        }

        private UserViewModel buildUserViewModel(ApplicationUser user)
        {
            return new UserViewModel()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                IsActive = user.IsActive,
                LockoutEnabled = user.LockoutEnabled,
                LastName = user.Surname,
                FullName = user.FirstName + " " + user.Surname,
                Roles = user.ROLES.Select(e => new IdNamePair<string>() { Id = e.Id, Name = e.Name }).ToList(),
                UserID = user.Id,
                Username = user.UserName,
                DateCreated = user.DateCreated,
                IsTemporaryPassword = user.IsTemporaryPassword,
                LastLoginDate = user.LastLoginDate,
                PhoneNumber = user.PhoneNumber,
                IsEmailVerified = user.EmailConfirmed
            };
        }


        public async Task<IPagedList<UserViewModel>> SearchUser(int page, int pageSize, string searchTerm, string roleId, string orderBy, bool orderByASC, bool includeInActive)
        {
            bool isEmptySearchTerm = string.IsNullOrWhiteSpace(searchTerm);
            bool isEmptyRoleId = string.IsNullOrWhiteSpace(roleId);
            orderBy = orderBy ?? "Username";
            var result = DbContext.Users.Where(e =>
                     ((isEmptySearchTerm || (e.FirstName.ToLower().Contains(searchTerm) || e.Surname.ToLower().Contains(searchTerm))) ||
                     (isEmptySearchTerm || e.UserName.ToLower().Contains(searchTerm))) &&
                     (isEmptyRoleId || e.Roles.Any(r => r.RoleId == roleId)) &&
                     (includeInActive || e.IsActive) &&
                     (e.UserName.ToLower() != Constants.SUPER_ADMIN_USERNAME.ToLower()))
                    .Select(user => buildUserViewModel(user))
                    .OrderBy(orderBy + " " + (orderByASC ? "ASC" : "DESC"));

            return await result.ToPagedListAsync(page, pageSize);
        }

        public async Task<bool> UserExists(string username)
        {
            return await DbContext.Users.AnyAsync(e => e.UserName.ToLower() == username.ToLower());
        }

    }


    public interface IApplicationUserRepository
    {
        Task<UserViewModel> GetUser(string userId);
        Task<List<UserViewModel>> GetUsers();
        Task<IPagedList<UserViewModel>> SearchUser(int page, int pageSize, string searchTerm, string roleId, string orderBy, bool orderByASC, bool includeInActive);
        Task<bool> UserExists(string username);
    }
}