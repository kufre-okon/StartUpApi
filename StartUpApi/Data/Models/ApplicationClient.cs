using StartUpApi.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace StartUpApi.Data.Models
{
    public class ApplicationClient
    {
        [Key]
        public string Id { get; set; }
        /// <summary>
        /// The Secret column is hashed so anyone has an access to the database will not be able to see the secrets
        /// </summary>
        [Required]
        public string Secret { get; set; }
        [Required]
        [MaxLength(120)]
        public string Name { get; set; }
        public ApplicationTypes ApplicationType { get; set; }
        /// <summary>
        /// Active column is very useful; if the system admin decided to deactivate this client, so any new requests asking for access token from this deactivated client will be rejected
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// The Refresh Token Life Time column is used to set when the refresh token (not the access token) will expire in minutes 
        /// it is nice feature because now you can control the expiry for refresh tokens for each client.
        /// </summary>
        public int RefreshTokenLifeTime { get; set; }
        /// <summary>
        /// Allowed Origin column is used to configure CORS and to set “Access-Control-Allow-Origin” on the back - end API
        /// </summary>
        [MaxLength(150)]
        public string AllowedOrigin { get; set; }
    }
}
