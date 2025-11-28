using Backend_CRUD.Domain.Entities;

namespace Backend_CRUD.Domain.Interfaces.Repositories
{
    public interface ISeriesRepository
    {
        Task<Serie> CreateSerieAsync(Serie serie);
        Task<(IEnumerable<Serie> Series, int TotalCount)> GetSeriesAsync(int page, int pageSize, string? search);
        Task<Serie?> GetSerieByIdAsync(int id);
        Task<Serie?> UpdateSerieAsync(Serie serie);
    }
}