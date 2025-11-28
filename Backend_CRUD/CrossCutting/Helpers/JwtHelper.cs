using Backend_CRUD.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend_CRUD.CrossCutting.Helpers
{
    public static class JwtHelper
    {
        public static async Task<IActionResult?> ValidateJwtToken(HttpRequest request, IJwtService jwtService, ILogger logger)
        {
            try
            {
                var token = ExtractTokenFromHeader(request);
                
                if (string.IsNullOrEmpty(token))
                {
                    logger.LogWarning("Token JWT no encontrado en la solicitud");
                    return new UnauthorizedObjectResult(new { message = "Token de autorización requerido" });
                }

                var principal = jwtService.ValidateToken(token);
                
                if (principal == null)
                {
                    logger.LogWarning("Token JWT inválido");
                    return new UnauthorizedObjectResult(new { message = "Token inválido o expirado" });
                }

                // Token válido - agregar información del usuario al contexto de la request
                request.HttpContext.User = principal;
                
                logger.LogInformation($"Usuario autenticado: {principal.Identity?.Name}");
                return null; // Token válido, continuar con la ejecución
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al validar token JWT");
                return new UnauthorizedObjectResult(new { message = "Error de autorización" });
            }
        }

        private static string? ExtractTokenFromHeader(HttpRequest request)
        {
            var authHeader = request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }
            return null;
        }

        public static string? GetUserIdFromToken(HttpRequest request)
        {
            var user = request.HttpContext.User;
            return user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }

        public static string? GetUserNameFromToken(HttpRequest request)
        {
            var user = request.HttpContext.User;
            return user?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        }
    }
}