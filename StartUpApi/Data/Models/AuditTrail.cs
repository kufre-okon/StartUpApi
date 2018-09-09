using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StartUpApi.Data.Models
{
    public class AuditTrail
    {
        [Key]
        public int ID { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser USER { get; set; }
        [StringLength(50)]
        public string Operation { get; set; }

        [StringLength(200)]
        public string OperationDescription { get; set; }

        public DateTime OperationDate { get; set; }
        [StringLength(600)]
        public string Error { get; set; }
    }
}
