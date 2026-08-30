using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Models;
using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity.API.Infrastructure.Services
{
    public interface ITokenProvider
    {
        string CreateToken(ApplicationUser user, IEnumerable<string> roles);
        string CreateRefreshToken();
    }

    public class TokenProvider(
        IOptions<JwtOptions> options) : ITokenProvider
    {
        public string CreateToken(ApplicationUser user, IEnumerable<string> roles)
        {
            var config = options.Value;

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.SigningKey));
            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.EmailVerified, user.EmailConfirmed.ToString()),
                ..roles.Select(r => new Claim(ClaimTypes.Role, r))
            ];

            var token = new JwtSecurityToken(
                issuer: config.Issuer,
                audience: config.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(config.ExpirationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            return Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));
        }
    }
}
