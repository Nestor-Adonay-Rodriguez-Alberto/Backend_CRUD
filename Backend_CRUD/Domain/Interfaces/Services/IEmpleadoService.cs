using Backend_CRUD.Domain.DTOs.Responses;
using Backend_CRUD.Domain.Entities;

namespace Backend_CRUD.Domain.Interfaces.Services
{
    public interface IEmpleadoService
    {
        Task<ResponseDTO<Empleado>> CreateEmpleadoAsync(Empleado empleado);
        Task<PaginatedResponseDTO<IEnumerable<Empleado>>> GetEmpleadosAsync(int page = 1, int pageSize = 10, string? search = null);
        Task<ResponseDTO<Empleado>> GetEmpleadoByIdAsync(int id);
        Task<ResponseDTO<Empleado>> UpdateEmpleadoAsync(int id, Empleado empleado);
    }
}