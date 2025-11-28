using Backend_CRUD.Application.DTOs.Responses;
using Backend_CRUD.CrossCutting.Configuration;
using Backend_CRUD.Domain.Entities;
using Backend_CRUD.Domain.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend_CRUD.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ITokenBlacklistService _blacklistService;

        public JwtService(IOptions<JwtSettings> jwtSettings, ITokenBlacklistService blacklistService)
        {
            _jwtSettings = jwtSettings.Value;
            _blacklistService = blacklistService;
        }

        public string GenerateToken(Empleado empleado)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var tokenId = Guid.NewGuid().ToString(); // JTI (JWT ID) para identificar el token
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, empleado.Id.ToString()),
                    new Claim(ClaimTypes.Name, empleado.Nombre),
                    new Claim("puesto", empleado.Puesto),
                    new Claim(JwtRegisteredClaimNames.Jti, tokenId),
                    new Claim(JwtRegisteredClaimNames.Iat, 
                        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                        ClaimValueTypes.Integer64)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
                
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                
                // Verificar si el token está en la blacklist
                var jwtToken = validatedToken as JwtSecurityToken;
                var jti = jwtToken?.Claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
                
                if (!string.IsNullOrEmpty(jti) && _blacklistService.IsTokenRevoked(jti))
                {
                    return null; // Token revocado
                }
                
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public LoginResponseDTO CreateLoginResponse(Empleado empleado, string token)
        {
            return new LoginResponseDTO
            {
                Token = token,
                Nombre = empleado.Nombre,
                Puesto = empleado.Puesto,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes)
            };
        }

        public string? GetTokenId(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                return jwtToken.Claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}