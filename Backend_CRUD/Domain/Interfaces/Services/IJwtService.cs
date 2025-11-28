using Backend_CRUD.Application.DTOs.Responses;
using Backend_CRUD.Domain.Entities;
using System.Security.Claims;

namespace Backend_CRUD.Domain.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(Empleado empleado);
        ClaimsPrincipal? ValidateToken(string token);
        LoginResponseDTO CreateLoginResponse(Empleado empleado, string token);
        string? GetTokenId(string token);
    }
}