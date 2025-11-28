using Backend_CRUD.Application.DTOs.Requests;
using Backend_CRUD.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Backend_CRUD.Presentation
{
    public class AuthFn
    {
        private readonly ILogger<AuthFn> _logger;
        private readonly IAuthService _authService;

        public AuthFn(ILogger<AuthFn> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [Function("Login")]
        public async Task<IActionResult> Login([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/login")] HttpRequest req)
        {
            _logger.LogInformation("Procesando solicitud de login.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                
                if (string.IsNullOrEmpty(requestBody))
                {
                    return new BadRequestObjectResult(new { message = "El cuerpo de la solicitud no puede estar vacío" });
                }

                var loginRequest = JsonConvert.DeserializeObject<LoginRequestDTO>(requestBody);
                
                if (loginRequest == null)
                {
                    return new BadRequestObjectResult(new { message = "Datos de login inválidos" });
                }

                var response = await _authService.LoginAsync(loginRequest);

                if (response.Status)
                {
                    return new OkObjectResult(response);
                }
                else
                {
                    return new UnauthorizedObjectResult(response);
                }
            }
            catch (JsonException)
            {
                return new BadRequestObjectResult(new { message = "Formato JSON inválido" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en login");
                return new StatusCodeResult(500);
            }
        }

        [Function("Logout")]
        public async Task<IActionResult> Logout([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/logout")] HttpRequest req)
        {
            _logger.LogInformation("Procesando solicitud de logout.");

            try
            {
                // Extraer token del header Authorization
                var authHeader = req.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                {
                    return new BadRequestObjectResult(new { message = "Token de autorización requerido" });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var response = await _authService.LogoutAsync(token);

                if (response.Status)
                {
                    return new OkObjectResult(response);
                }
                else
                {
                    return new BadRequestObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en logout");
                return new StatusCodeResult(500);
            }
        }
    }
}