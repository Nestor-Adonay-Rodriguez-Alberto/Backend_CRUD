using Backend_CRUD.Domain.DTOs.Responses;
using Backend_CRUD.Domain.Entities;
using Backend_CRUD.Domain.Interfaces.Repositories;
using Backend_CRUD.Domain.Interfaces.Services;

namespace Backend_CRUD.Application.Services
{
    public class SeriesService : ISeriesService
    {
        private readonly ISeriesRepository _seriesRepository;

        public SeriesService(ISeriesRepository seriesRepository)
        {
            _seriesRepository = seriesRepository;
        }




        // CREAR:
        public async Task<ResponseDTO<Serie>> CreateSerieAsync(Serie serie)
        {
            try
            {

                var createdSerie = await _seriesRepository.CreateSerieAsync(serie);

                return new ResponseDTO<Serie>
                {
                    Status = true,
                    Message = "Serie creada exitosamente",
                    Data = createdSerie
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<Serie>
                {
                    Status = false,
                    Message = $"Error al crear la serie: {ex.Message}",
                    Data = null
                };
            }
        }


        // OBTENER LISTADO:
        public async Task<PaginatedResponseDTO<IEnumerable<Serie>>> GetSeriesAsync(int page = 1, int pageSize = 10, string? search = null)
        {
            try
            {
                // Validar parámetros
                if (page <= 0) page = 1;
                if (pageSize <= 0) pageSize = 10;
                if (pageSize > 100) pageSize = 100; // Límite máximo para evitar sobrecarga

                var (series, totalCount) = await _seriesRepository.GetSeriesAsync(page, pageSize, search);

                return new PaginatedResponseDTO<IEnumerable<Serie>>
                {
                    Status = true,
                    Message = "Series obtenidas exitosamente",
                    Data = series,
                    Count = totalCount
                };
            }
            catch (Exception ex)
            {
                return new PaginatedResponseDTO<IEnumerable<Serie>>
                {
                    Status = false,
                    Message = $"Error al obtener las series: {ex.Message}",
                    Data = null,
                    Count = 0
                };
            }
        }


        // OBTENER POR ID:
        public async Task<ResponseDTO<Serie>> GetSerieByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new ResponseDTO<Serie>
                    {
                        Status = false,
                        Message = "El ID debe ser mayor a 0",
                        Data = null
                    };
                }

                var serie = await _seriesRepository.GetSerieByIdAsync(id);

                if (serie == null)
                {
                    return new ResponseDTO<Serie>
                    {
                        Status = false,
                        Message = "Serie no encontrada",
                        Data = null
                    };
                }

                return new ResponseDTO<Serie>
                {
                    Status = true,
                    Message = "Serie obtenida exitosamente",
                    Data = serie
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<Serie>
                {
                    Status = false,
                    Message = $"Error al obtener la serie: {ex.Message}",
                    Data = null
                };
            }
        }


        // ACTUALIZAR:
        public async Task<ResponseDTO<Serie>> UpdateSerieAsync(int id, Serie serie)
        {
            try
            {
                if (id <= 0)
                {
                    return new ResponseDTO<Serie>
                    {
                        Status = false,
                        Message = "El ID debe ser mayor a 0",
                        Data = null
                    };
                }

                if (string.IsNullOrEmpty(serie.Titulo))
                {
                    return new ResponseDTO<Serie>
                    {
                        Status = false,
                        Message = "El título es requerido",
                        Data = null
                    };
                }

                if (serie.Temporadas <= 0)
                {
                    return new ResponseDTO<Serie>
                    {
                        Status = false,
                        Message = "El número de temporadas debe ser mayor a 0",
                        Data = null
                    };
                }

                // Asegurar que el ID del objeto coincida con el parámetro
                serie.Id = id;

                var updatedSerie = await _seriesRepository.UpdateSerieAsync(serie);

                if (updatedSerie == null)
                {
                    return new ResponseDTO<Serie>
                    {
                        Status = false,
                        Message = "Serie no encontrada",
                        Data = null
                    };
                }

                return new ResponseDTO<Serie>
                {
                    Status = true,
                    Message = "Serie actualizada exitosamente",
                    Data = updatedSerie
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<Serie>
                {
                    Status = false,
                    Message = $"Error al actualizar la serie: {ex.Message}",
                    Data = null
                };
            }
        }
   
    }
}