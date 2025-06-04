using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FutbolYa.WebAPI.Helpers
{
    public static class JwtHelper
    {
        public static string GenerarToken(string usuarioId, string rol, string key)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId),
                new Claim(ClaimTypes.Role, rol)
            };

            var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
