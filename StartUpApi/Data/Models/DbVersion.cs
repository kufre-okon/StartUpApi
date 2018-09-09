using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StartUpApi.Data.Models
{
    public class DbVersion
    {
        [Key]
        public int Id { get; set; }        
        public int VersionNumber { get; set; }
    }
}
