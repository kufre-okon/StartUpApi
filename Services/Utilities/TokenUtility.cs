using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Utilities
{
    public class TokenUtility
    {
        /// <summary>
        /// Generate JwtSecurity Token
        /// </summary>
        /// <param name="appConfigSection"></param>
        /// <param name="claims"> the user's claims, for example new Claim[] { new Claim(ClaimTypes.Name, "The username")</param>
        /// <returns></returns>
        public static JwtSecurityToken GenerateJwtSecurityToken(IConfigurationSection appConfigSection, IEnumerable<Claim> claims)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfigSection["Key"]));
            var signingCredential = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                 issuer: appConfigSection["AppIssuer"],
                 claims: claims,
                 audience: appConfigSection["AppIssuer"],
                 notBefore: DateTime.UtcNow,
                 expires: DateTime.UtcNow.AddMinutes(3), // DateTime.UtcNow.AddHours(int.Parse(appConfigSection["TokenExpires"])),
                  signingCredentials: signingCredential
                );

            return jwtSecurityToken;
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public static IEnumerable<Claim> GenerateClaims(string username, string userId)
        {
            var claims = new Claim[] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.NameIdentifier, userId) };
            return claims;
        }

        public static ClaimsPrincipal GetPrincipalFromExpiredToken(IConfigurationSection appConfigSection, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, BuildTokenValidationParameters(appConfigSection), out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.CurrentCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public static TokenValidationParameters BuildTokenValidationParameters(IConfigurationSection appConfigSection)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfigSection["Key"])),
                ValidAudience = appConfigSection["AppIssuer"],
                ValidIssuer = appConfigSection["AppIssuer"]
            };

            return tokenValidationParameters;
        }
    }
}
