using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StartUpApi.Data.DTO
{
    public class PasswordResetRequest
    {
        public string UserId { get; set; }
        public string NewPassword { get; set; }
    }
}
