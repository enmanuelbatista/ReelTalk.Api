using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ReelTalk.Api.Modelos;

namespace ReelTalk.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerarToken(Usuario usuario)
        {
            // 1. Los "Claims" son afirmaciones sobre el usuario, guardadas dentro del token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            // 2. Convertimos la llave secreta de texto a bytes, y creamos el objeto de llave
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            // 3. Las credenciales de firma: la llave + el algoritmo a usar
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4. Armamos el token con todos sus datos
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["Jwt:ExpiraEnMinutos"]!)
                ),
                signingCredentials: credenciales
            );

            // 5. Convertimos el objeto token a un string real, el que se manda al cliente
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
