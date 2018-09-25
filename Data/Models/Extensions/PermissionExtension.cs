using Data.DTO;

namespace Data.Models.Extensions
{
    public static class PermissionExtension
    {

        public static PermissionDto ToPermissionDto(this Permission perm)
        {
            return new PermissionDto()
            {
                displayName = perm.DisplayName,
                level = perm.Level,
                name = perm.Name,
                parentId = perm.ParentId,
                permissionID = perm.ID,
                ParentName = perm.PARENTPERMISSION?.Name
            };
        }

        public static Permission ToPermission(this PermissionDto vm)
        {
            return new Permission()
            {
                DisplayName = vm.displayName,
                Level = vm.level,
                Name = vm.name,
                ParentId = vm.parentId,
                ID = vm.permissionID,               
            };
        }
    }
}
