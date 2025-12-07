using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OceanTeseach.API.Models;

namespace OceanTeseach.API.Services
{
    public class JwtService : IJwtService
    {
        private readonly string _secret;
        private readonly int _expiryMinutes;

        public JwtService(IConfiguration configuration)
        {
            _secret = configuration["Jwt:Secret"] ?? throw new Exception("Jwt:Secret not configured");
            _expiryMinutes = int.Parse(configuration["Jwt:ExpiryMinutes"] ?? "1440");
        }

        public string GenerateToken(User user)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_expiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }
    }
}
