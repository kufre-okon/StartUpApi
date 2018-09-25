using Data.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class UserSession
    {
        [Key]
        public int Id { get; set; }
        public string userId { get; set; }
        [ForeignKey("userId")]
        public virtual ApplicationUser USER { get; set; }
        public DateTime startDate { get; set; }
        public DateTime? endTime { get; set; }
        public ApplicationTypes appMode { get; set; }
    }
}
