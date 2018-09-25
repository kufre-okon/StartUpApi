using Data.DTO;
using System;
using System.Linq;

namespace Data.Models.Extensions
{
    public static class RoleExtension
    {

        public static RoleDto ToRoleDto(this ApplicationRole role)
        {
            return new RoleDto()
            {
                Id = role.Id,
                DateCreated = role.DateCreated,
                Default = role.Default,
                Description = role.Description,
                IsStatic = role.IsStatic,
                Name = role.Name,
                AssignedPermissions = role.ROLEPERMISSIONS.Select(e => e.permissionId).ToList()
            };
        }

        public static RoleListDto ToRoleListDto(this ApplicationRole role)
        {
            return new RoleListDto()
            {
                Id = role.Id,               
                Description = role.Description,
                Name = role.Name
            };
        }

        public static ApplicationRole ToApplicationRole(this RoleDto vm)
        {
            return new ApplicationRole()
            {
                Id = vm.Id ?? Guid.NewGuid().ToString(),
                DateCreated = vm.DateCreated,
                Default = vm.Default,
                Description = vm.Description,
                IsStatic = vm.IsStatic,
                Name = vm.Name,
            };
        }
    }
}
