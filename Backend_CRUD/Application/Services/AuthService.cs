using Backend_CRUD.Application.DTOs.Requests;
using Backend_CRUD.Application.DTOs.Responses;
using Backend_CRUD.Domain.DTOs.Responses;
using Backend_CRUD.Domain.Interfaces.Repositories;
using Backend_CRUD.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Backend_CRUD.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IEmpleadoRepository _empleadoRepository;
        private readonly IJwtService _jwtService;
        private readonly ITokenBlacklistService _blacklistService;

        public AuthService(IEmpleadoRepository empleadoRepository, IJwtService jwtService, ITokenBlacklistService blacklistService)
        {
            _empleadoRepository = empleadoRepository;
            _jwtService = jwtService;
            _blacklistService = blacklistService;
        }

        public async Task<ResponseDTO<LoginResponseDTO>> LoginAsync(LoginRequestDTO loginRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(loginRequest.Nombre) || string.IsNullOrEmpty(loginRequest.Contraseña))
                {
                    return new ResponseDTO<LoginResponseDTO>
                    {
                        Status = false,
                        Message = "Nombre y contraseña son requeridos",
                        Data = null
                    };
                }

                // Buscar empleado por nombre y contraseña
                var empleado = await _empleadoRepository.GetEmpleadoByCredentialsAsync(loginRequest.Nombre, loginRequest.Contraseña);

                if (empleado == null)
                {
                    return new ResponseDTO<LoginResponseDTO>
                    {
                        Status = false,
                        Message = "Credenciales inválidas",
                        Data = null
                    };
                }

                // Generar token JWT
                var token = _jwtService.GenerateToken(empleado);
                var loginResponse = _jwtService.CreateLoginResponse(empleado, token);

                return new ResponseDTO<LoginResponseDTO>
                {
                    Status = true,
                    Message = "Login exitoso",
                    Data = loginResponse
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<LoginResponseDTO>
                {
                    Status = false,
                    Message = $"Error en el login: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseDTO<string>> LogoutAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return new ResponseDTO<string>
                    {
                        Status = false,
                        Message = "Token requerido",
                        Data = null
                    };
                }

                // Validar que el token sea válido antes de revocarlo
                var principal = _jwtService.ValidateToken(token);
                if (principal == null)
                {
                    return new ResponseDTO<string>
                    {
                        Status = false,
                        Message = "Token inválido o expirado",
                        Data = null
                    };
                }

                // Obtener el ID del token (JTI) y agregarlo a la blacklist
                var tokenId = _jwtService.GetTokenId(token);
                if (!string.IsNullOrEmpty(tokenId))
                {
                    _blacklistService.RevokeToken(tokenId);
                }

                return new ResponseDTO<string>
                {
                    Status = true,
                    Message = "Logout exitoso",
                    Data = "Sesión cerrada correctamente"
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<string>
                {
                    Status = false,
                    Message = $"Error en el logout: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}