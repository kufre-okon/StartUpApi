namespace StartUpApi.ViewModels
{
    public class PermissionViewModel 
    {
        public int permissionID { get; set; }
        public string name { get; set; }
        public int? parentid { get; set; }
        public string ParentName { get; set; }
        public int? level { get; set; }
        public string displayname { get; set; }
    }
}