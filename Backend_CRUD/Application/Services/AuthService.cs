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

        public AuthService(IEmpleadoRepository empleadoRepository, IJwtService jwtService)
        {
            _empleadoRepository = empleadoRepository;
            _jwtService = jwtService;
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
    }
}