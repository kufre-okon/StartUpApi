﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.DTO
{
    public class UserCountDTO
    {
        public UserCountDTO()
        {           
        }      
        public int ActiveUsers { get; set; }
      
        public int InActiveUsers { get; set; }        
    }
}
