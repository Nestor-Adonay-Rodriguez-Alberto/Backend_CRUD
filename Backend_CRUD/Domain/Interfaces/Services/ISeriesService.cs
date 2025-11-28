using Backend_CRUD.Domain.DTOs.Responses;
using Backend_CRUD.Domain.Entities;

namespace Backend_CRUD.Domain.Interfaces.Services
{
    public interface ISeriesService
    {
        Task<ResponseDTO<Serie>> CreateSerieAsync(Serie serie);
        Task<PaginatedResponseDTO<IEnumerable<Serie>>> GetSeriesAsync(int page = 1, int pageSize = 10, string? search = null);
        Task<ResponseDTO<Serie>> GetSerieByIdAsync(int id);
        Task<ResponseDTO<Serie>> UpdateSerieAsync(int id, Serie serie);
    }
}