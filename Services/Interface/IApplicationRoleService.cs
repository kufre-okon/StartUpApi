using Data.DTO;
using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IApplicationRoleService
    {
        void AddRole(RoleDto role);
        Task<RoleDto> GetRole(string Id);
        void UpdateRole(ApplicationRole menu);
        Task<List<RoleDto>> GetList();
        Task<List<RoleListDto>> GetListLightWeight();
        Task<List<RoleDto>> GetByPermission( int permissionId);
        void UpdateRolePermission(string roleId, List<int> permissionIds);
        Task UpdateRoleUsers(string roleId, List<string> userIds);
        Task<List<PermissionDto>> GetRolePermissions(string roleId);
        Task<List<UserDto>> GetRoleUsers(string id);
        Task Delete(string id);
    }
}
