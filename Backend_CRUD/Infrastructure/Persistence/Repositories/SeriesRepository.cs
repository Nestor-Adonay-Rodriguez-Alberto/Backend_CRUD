using Backend_CRUD.Domain.Entities;
using Backend_CRUD.Domain.Interfaces.Repositories;
using Backend_CRUD.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;


namespace Backend_CRUD.Infrastructure.Persistence.Repositories
{
    public class SeriesRepository : ISeriesRepository
    {
        private readonly AppDbContext _context;

        public SeriesRepository(AppDbContext context)
        {
            _context = context;
        }




        // CREAR:
        public async Task<Serie> CreateSerieAsync(Serie serie)
        {
            _context.Series.Add(serie);
            await _context.SaveChangesAsync();
            return serie;
        }


        // OBTENER LISTADO:
        public async Task<(IEnumerable<Serie> Series, int TotalCount)> GetSeriesAsync(int page, int pageSize, string? search)
        {
            var query = _context.Series.AsQueryable();

            // Aplicar filtro de búsqueda si se proporciona
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.Titulo.Contains(search));
            }

            // Obtener el total de registros antes de la paginación
            var totalCount = await query.CountAsync();

            // Aplicar paginación
            var series = await query
                .OrderBy(s => s.Titulo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (series, totalCount);
        }


        // OBTENER POR ID:
        public async Task<Serie?> GetSerieByIdAsync(int id)
        {
            return await _context.Series.FindAsync(id);
        }


        // ACTUALIZAR:
        public async Task<Serie?> UpdateSerieAsync(Serie serie)
        {
            var existingSerie = await _context.Series.FindAsync(serie.Id);
            
            if (existingSerie == null)
                return null;

            existingSerie.Titulo = serie.Titulo;
            existingSerie.Temporadas = serie.Temporadas;

            await _context.SaveChangesAsync();
            return existingSerie;
        }
    }
}