using JWT.Security.Security;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JWT.Security.Services
{
    public class GenerateTokenService
    {
        /// <summary>
        /// Method for generating token. 
        /// </summary>
        /// <param name="userId">Id of user saved in JSON Users File </param>
        /// <param name="fullName">In fact it is an optional parameter</param>
        /// <param name="lifetimeInMinutes">Lifetime of token from NOW.</param>
        /// <param name="securityTokenDescriptor">Output param for geting values from security token descriptor.</param>
        /// <returns></returns>
        public static string CreateToken(string userId, string fullName, int lifetimeInMinutes,out SecurityTokenDescriptor securityTokenDescriptor)
        {
            // Create the JWT
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim("name", fullName)
                // And any other bit of (session) data you want....
            });

            var now = DateTime.UtcNow;
            var tokenHandler = new JwtSecurityTokenHandler();

            securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Issuer = SecurityConfiguration.TokenIssuer,
                Audience = SecurityConfiguration.TokenAudience,
                SigningCredentials = SecurityConfiguration.SigningCredentials,
                IssuedAt = now,
                Expires = now.AddMinutes(lifetimeInMinutes)
            };

            var token = tokenHandler.CreateToken(securityTokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
