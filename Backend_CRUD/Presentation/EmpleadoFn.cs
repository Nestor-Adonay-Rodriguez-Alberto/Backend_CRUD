using Backend_CRUD.Domain.Entities;
using Backend_CRUD.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Backend_CRUD.Presentation
{
    public class EmpleadoFn
    {
        private readonly ILogger<EmpleadoFn> _logger;
        private readonly IEmpleadoService _empleadoService;

        public EmpleadoFn(ILogger<EmpleadoFn> logger, IEmpleadoService empleadoService)
        {
            _logger = logger;
            _empleadoService = empleadoService;
        }

        // OBTENER LISTADO:
        [Function("GetEmpleados")]
        public async Task<IActionResult> GetEmpleados([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "empleados/get-all")] HttpRequest req)
        {
            _logger.LogInformation("Procesando solicitud para obtener listado de empleados.");

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

                var response = await _empleadoService.GetEmpleadosAsync(page, pageSize, searchFilter);

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener empleados");
                return new StatusCodeResult(500);
            }
        }

        // OBTENER POR ID:
        [Function("GetEmpleadoById")]
        public async Task<IActionResult> GetEmpleadoById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "empleados/{id}")] HttpRequest req, int id)
        {
            _logger.LogInformation($"Procesando solicitud para obtener empleado con ID: {id}.");

            try
            {
                var response = await _empleadoService.GetEmpleadoByIdAsync(id);

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
                _logger.LogError(ex, $"Error inesperado al obtener empleado con ID: {id}");
                return new StatusCodeResult(500);
            }
        }

        // ACTUALIZAR:
        [Function("UpdateEmpleado")]
        public async Task<IActionResult> UpdateEmpleado([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "empleados/{id}")] HttpRequest req, int id)
        {
            _logger.LogInformation($"Procesando solicitud para actualizar empleado con ID: {id}.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                
                if (string.IsNullOrEmpty(requestBody))
                {
                    return new BadRequestObjectResult(new { message = "El cuerpo de la solicitud no puede estar vacío" });
                }

                var empleado = JsonConvert.DeserializeObject<Empleado>(requestBody);
                
                if (empleado == null)
                {
                    return new BadRequestObjectResult(new { message = "Datos de empleado inválidos" });
                }

                var response = await _empleadoService.UpdateEmpleadoAsync(id, empleado);

                if (response.Status)
                {
                    return new OkObjectResult(response);
                }
                else
                {
                    if (response.Message.Contains("no encontrado"))
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
                _logger.LogError(ex, $"Error inesperado al actualizar empleado con ID: {id}");
                return new StatusCodeResult(500);
            }
        }

        // CREAR:
        [Function("CreateEmpleado")]
        public async Task<IActionResult> CreateEmpleado([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "empleados")] HttpRequest req)
        {
            _logger.LogInformation("Procesando solicitud para crear un nuevo empleado.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                
                if (string.IsNullOrEmpty(requestBody))
                {
                    return new BadRequestObjectResult(new { message = "El cuerpo de la solicitud no puede estar vacío" });
                }

                var empleado = JsonConvert.DeserializeObject<Empleado>(requestBody);
                
                if (empleado == null)
                {
                    return new BadRequestObjectResult(new { message = "Datos de empleado inválidos" });
                }

                var response = await _empleadoService.CreateEmpleadoAsync(empleado);

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
                _logger.LogError(ex, "Error inesperado al crear empleado");
                return new StatusCodeResult(500);
            }
        }
    }
}