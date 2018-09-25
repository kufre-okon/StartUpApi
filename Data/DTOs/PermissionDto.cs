namespace Data.DTO
{
    public class PermissionDto 
    {
        public int permissionID { get; set; }
        public string name { get; set; }
        public int? parentId { get; set; }
        public string ParentName { get; set; }
        public int? level { get; set; }
        public string displayName { get; set; }
    }
}