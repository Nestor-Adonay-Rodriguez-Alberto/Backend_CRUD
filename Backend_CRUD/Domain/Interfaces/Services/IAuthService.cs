using Backend_CRUD.Application.DTOs.Requests;
using Backend_CRUD.Application.DTOs.Responses;
using Backend_CRUD.Domain.DTOs.Responses;

namespace Backend_CRUD.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ResponseDTO<LoginResponseDTO>> LoginAsync(LoginRequestDTO loginRequest);
    }
}