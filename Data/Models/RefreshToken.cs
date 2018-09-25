using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class RefreshToken
    {
        /// <summary>
        /// Id column contains hashed value of the refresh token id, the API consumer will receive and send the plain refresh token Id
        /// </summary>
        [Key]
        public string Id { get; set; }
        /// <summary>
        /// Subject column indicates to which user this refresh token belongs, and the same applied for Client Id column
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Subject { get; set; }
        [Required]
        [MaxLength(120)]
        public string ClientId { get; set; }
        public DateTime IssuedUtc { get; set; }
        [MaxLength(128)]
        public string LoginSource { get; set; }
        public DateTime ExpiresUtc { get; set; }
        /// <summary>
        /// Protected Ticket column contains magical signed string which contains a serialized representation for the ticket for specific user, 
        /// in other words it contains all the claims and ticket properties for this user. 
        /// </summary>
        [Required]
        public string ProtectedTicket { get; set; }
    }
}
