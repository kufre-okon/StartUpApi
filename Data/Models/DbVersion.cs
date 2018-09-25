using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class DbVersion
    {
        [Key]
        public int Id { get; set; }        
        public int VersionNumber { get; set; }
    }
}
