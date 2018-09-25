using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.DTO
{
    public class RoleDto
    {
        /// <summary>
        /// Readonly
        /// </summary>
        public string Id { get; set; }
        
        public string Name { get; set; }
       
        public bool IsStatic { get; set; }
        public bool Default { get; set; }
        /// <summary>
        /// Optional
        /// </summary>        
        public string Description { get; set; }
        
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// List of assigned permision IDs
        /// </summary>
        public List<int> AssignedPermissions { get; set; } = new List<int>();
    }

    public class RoleListDto
    {
        public string Id { get; set; }

        public string Name { get; set; }
               
        public string Description { get; set; }
    }
}