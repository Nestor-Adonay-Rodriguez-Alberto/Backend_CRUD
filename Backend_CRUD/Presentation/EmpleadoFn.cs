using Backend_CRUD.CrossCutting.Helpers;
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
        private readonly IJwtService _jwtService;

        public EmpleadoFn(ILogger<EmpleadoFn> logger, IEmpleadoService empleadoService, IJwtService jwtService)
        {
            _logger = logger;
            _empleadoService = empleadoService;
            _jwtService = jwtService;
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
        public async Task<IActionResult> GetEmpleadoById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "empleados/ById/{id}")] HttpRequest req, int id)
        {
            _logger.LogInformation("Procesando solicitud para obtener empleado por ID.");

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
                _logger.LogError(ex, "Error inesperado al obtener empleado por ID");
                return new StatusCodeResult(500);
            }
        }


        // ACTUALIZAR:
        [Function("UpdateEmpleado")]
        public async Task<IActionResult> UpdateEmpleado([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "empleados/{id}")] HttpRequest req)
        {
            _logger.LogInformation("Procesando solicitud para actualizar empleado.");

            // Validar JWT
            var jwtValidation = await JwtHelper.ValidateJwtToken(req, _jwtService, _logger);
            if (jwtValidation != null) return jwtValidation;

            try
            {
                // Extraer ID de la ruta manualmente
                var routeValues = req.RouteValues;
                if (!routeValues.TryGetValue("id", out var idValue) || !int.TryParse(idValue?.ToString(), out var id))
                {
                    return new BadRequestObjectResult(new { message = "ID inválido" });
                }

                _logger.LogInformation($"ID extraído: {id}");

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
                _logger.LogError(ex, "Error inesperado al actualizar empleado");
                return new StatusCodeResult(500);
            }
        }


        // CREAR:
        [Function("CreateEmpleado")]
        public async Task<IActionResult> CreateEmpleado([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "empleados")] HttpRequest req)
        {
            _logger.LogInformation("Procesando solicitud para crear un nuevo empleado.");

            // Validar JWT
            var jwtValidation = await JwtHelper.ValidateJwtToken(req, _jwtService, _logger);
            if (jwtValidation != null) return jwtValidation;

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