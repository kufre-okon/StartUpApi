using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StartUpApi.ViewModels
{
    public class RoleViewModel
    {
        /// <summary>
        /// Readonly
        /// </summary>
        public string Id { get; set; }
        
        public string Name { get; set; }
        /// <summary>
        /// Readonly
        /// </summary>
        public int? TenantId { get; set; }
        /// <summary>
        /// Readonly
        /// </summary>
        public string TenantName { get; set; }
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
}