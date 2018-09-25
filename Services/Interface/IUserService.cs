using System.Collections.Generic;
using System.Threading.Tasks;
using Pager.Interface;
using Data.DTO;
using Data.Models;

namespace Services.Interface
{
    public interface IUserService
    {
        Task<IPaginate<UserDto>> SearchUser(int page, int pageSize, string searchTerm, string roleId, string orderBy, bool orderByASC, bool includeInActive);
        Task<IEnumerable<ApplicationUser>> GetUsersByRole(string roleId);
        Task<UserCountDTO> CountUsers();
        Task<UserDto> GetUser(string userId);
        void DeleteUser(ApplicationUser user);
        void DeleteUser(string userId);
        Task<List<PermissionDto>> GetUserPermissions(string userId, bool isSuperAdmin = false);
        Task ChangePassword(PasswordChangeRequest data, string userId);
        void UpdateUser(UserDto user);
        Task UpdateUserRole(string userId, List<string> roles);
        Task ToggleUserAccount(string userId, bool status);
        Task ResetPassword(PasswordResetRequest model);
        Task ResetSpecialPerimssions(string userId);
        Task UpdateUserSpecialPermissions(string userId, List<UserSpecialPermissionData> data);

        Task<UserDto> CreateUser(UserDto user);
        Task<ApplicationUser> FindByUsername(string username);
        Task<bool> UserExists(string username);
    }
}
