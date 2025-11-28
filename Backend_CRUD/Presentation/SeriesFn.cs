using Backend_CRUD.Domain.Entities;
using Backend_CRUD.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Backend_CRUD.Presentation
{
    public class SeriesFn
    {
        private readonly ILogger<SeriesFn> _logger;
        private readonly ISeriesService _seriesService;

        public SeriesFn(ILogger<SeriesFn> logger, ISeriesService seriesService)
        {
            _logger = logger;
            _seriesService = seriesService;
        }



        // OBTENER LISTADO:
        [Function("GetSeries")]
        public async Task<IActionResult> GetSeries([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series/get-all")] HttpRequest req)
        {
            _logger.LogInformation("Procesando solicitud para obtener listado de series.");

            try
            {
                // Obtener parámetros de query
                var pageParam = req.Query["page"].ToString();
                var pageSizeParam = req.Query["pageSize"].ToString();
                var search = req.Query["search"].ToString();

                // Parsear parámetros con valores por defecto
                int page = string.IsNullOrEmpty(pageParam) ? 1 : int.TryParse(pageParam, out var p) ? p : 1;
                int pageSize = string.IsNullOrEmpty(pageSizeParam) ? 10 : int.TryParse(pageSizeParam, out var ps) ? ps : 10;
                string? searchFilter = string.IsNullOrEmpty(search) ? null : search;

                var response = await _seriesService.GetSeriesAsync(page, pageSize, searchFilter);

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener series");
                return new StatusCodeResult(500);
            }
        }


        // OBTENER POR ID:
        [Function("GetSerieById")]
        public async Task<IActionResult> GetSerieById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series/{id}")] HttpRequest req, int id)
        {
            _logger.LogInformation($"Procesando solicitud para obtener serie con ID: {id}.");

            try
            {
                var response = await _seriesService.GetSerieByIdAsync(id);

                if (response.Status)
                {
                    return new OkObjectResult(response);
                }
                else
                {
                    return new NotFoundObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error inesperado al obtener serie con ID: {id}");
                return new StatusCodeResult(500);
            }
        }


        // ACTUALIZAR:
        [Function("UpdateSerie")]
        public async Task<IActionResult> UpdateSerie([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "series/{id}")] HttpRequest req, int id)
        {
            _logger.LogInformation($"Procesando solicitud para actualizar serie con ID: {id}.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                
                if (string.IsNullOrEmpty(requestBody))
                {
                    return new BadRequestObjectResult(new { message = "El cuerpo de la solicitud no puede estar vacío" });
                }

                var serie = JsonConvert.DeserializeObject<Serie>(requestBody);
                
                if (serie == null)
                {
                    return new BadRequestObjectResult(new { message = "Datos de serie inválidos" });
                }

                var response = await _seriesService.UpdateSerieAsync(id, serie);

                if (response.Status)
                {
                    return new OkObjectResult(response);
                }
                else
                {
                    if (response.Message.Contains("no encontrada"))
                    {
                        return new NotFoundObjectResult(response);
                    }
                    return new BadRequestObjectResult(response);
                }
            }
            catch (JsonException)
            {
                return new BadRequestObjectResult(new { message = "Formato JSON inválido" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error inesperado al actualizar serie con ID: {id}");
                return new StatusCodeResult(500);
            }
        }


        // CREAR:
        [Function("CreateSerie")]
        public async Task<IActionResult> CreateSerie([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "series")] HttpRequest req)
        {
            _logger.LogInformation("Procesando solicitud para crear una nueva serie.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                
                if (string.IsNullOrEmpty(requestBody))
                {
                    return new BadRequestObjectResult(new { message = "El cuerpo de la solicitud no puede estar vacío" });
                }

                var serie = JsonConvert.DeserializeObject<Serie>(requestBody);
                
                if (serie == null)
                {
                    return new BadRequestObjectResult(new { message = "Datos de serie inválidos" });
                }

                var response = await _seriesService.CreateSerieAsync(serie);

                if (response.Status)
                {
                    return new OkObjectResult(response);
                }
                else
                {
                    return new BadRequestObjectResult(response);
                }
            }
            catch (JsonException)
            {
                return new BadRequestObjectResult(new { message = "Formato JSON inválido" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear serie");
                return new StatusCodeResult(500);
            }
        }



    }
}