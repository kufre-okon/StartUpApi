using StartUpApi.Data.DTO;
using System;
using System.Linq;

namespace StartUpApi.Data.Models.Extensions
{
    public static class UserExtension
    {
        public static UserDto BuildUserViewModel(ApplicationUser user)
        {
            return new UserDto()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                IsActive = user.IsActive,
                LockoutEnabled = user.LockoutEnabled && (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow),
                LastName = user.Surname,
                FullName = user.FirstName + " " + user.Surname,
                Roles = user.USERROLES.Select(e => new IdNamePair<string>() { Id = e.RoleId, Name = e.ROLE.Name }).ToList(),
                UserID = user.Id,
                Username = user.UserName,
                DateCreated = user.DateCreated,
                IsTemporaryPassword = user.IsTemporaryPassword,
                LastLoginDate = user.LastLoginDate,
                PhoneNumber = user.PhoneNumber,
                IsEmailVerified = user.EmailConfirmed,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Password = user.PasswordHash
            };
        }

        public static UserDto ToUserDto(this ApplicationUser user)
        {
            return BuildUserViewModel(user);
        }

        public static ApplicationUser ToApplicationUser(this UserDto vm)
        {
            var user = new ApplicationUser()
            {
                Email = vm.Email,
                FirstName = vm.FirstName,
                IsActive = vm.IsActive,
                LockoutEnabled = vm.LockoutEnabled,
                Surname = vm.LastName,
                Id = vm.UserID ?? Guid.NewGuid().ToString(),
                UserName = vm.Username,
                DateCreated = vm.DateCreated ?? DateTime.UtcNow,
                IsTemporaryPassword = vm.IsTemporaryPassword,
                LastLoginDate = vm.LastLoginDate,
                PhoneNumber = vm.PhoneNumber,
                ProfilePictureUrl = vm.ProfilePictureUrl,
                PasswordHash = vm.Password
            };
            if (vm.Roles.Count > 0)
                user.USERROLES = vm.Roles.Select(e => new ApplicationUserRole() { RoleId = e.Id, UserId = vm.UserID }).ToList();
            return user;
        }
    }
}
