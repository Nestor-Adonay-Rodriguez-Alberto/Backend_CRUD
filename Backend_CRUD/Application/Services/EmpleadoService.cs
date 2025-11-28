using Backend_CRUD.Application.DTOs.Responses;
using Backend_CRUD.Domain.Entities;
using Backend_CRUD.Domain.Interfaces.Repositories;
using Backend_CRUD.Domain.Interfaces.Services;

namespace Backend_CRUD.Application.Services
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _empleadoRepository;

        public EmpleadoService(IEmpleadoRepository empleadoRepository)
        {
            _empleadoRepository = empleadoRepository;
        }

        // CREAR:
        public async Task<ResponseDTO<Empleado>> CreateEmpleadoAsync(Empleado empleado)
        {
            try
            {
                if (string.IsNullOrEmpty(empleado.Nombre))
                {
                    return new ResponseDTO<Empleado>
                    {
                        Status = false,
                        Message = "El nombre es requerido",
                        Data = null
                    };
                }

                if (string.IsNullOrEmpty(empleado.Puesto))
                {
                    return new ResponseDTO<Empleado>
                    {
                        Status = false,
                        Message = "El puesto es requerido",
                        Data = null
                    };
                }

                if (string.IsNullOrEmpty(empleado.Contraseña))
                {
                    return new ResponseDTO<Empleado>
                    {
                        Status = false,
                        Message = "La contraseña es requerida",
                        Data = null
                    };
                }

                var createdEmpleado = await _empleadoRepository.CreateEmpleadoAsync(empleado);

                return new ResponseDTO<Empleado>
                {
                    Status = true,
                    Message = "Empleado creado exitosamente",
                    Data = createdEmpleado
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<Empleado>
                {
                    Status = false,
                    Message = $"Error al crear el empleado: {ex.Message}",
                    Data = null
                };
            }
        }

        // OBTENER LISTADO:
        public async Task<PaginatedResponseDTO<IEnumerable<Empleado>>> GetEmpleadosAsync(int page = 1, int pageSize = 10, string? search = null)
        {
            try
            {
                // Validar parámetros
                if (page <= 0) page = 1;
                if (pageSize <= 0) pageSize = 10;
                if (pageSize > 100) pageSize = 100; // Límite máximo para evitar sobrecarga

                var (empleados, totalCount) = await _empleadoRepository.GetEmpleadosAsync(page, pageSize, search);

                return new PaginatedResponseDTO<IEnumerable<Empleado>>
                {
                    Status = true,
                    Message = "Empleados obtenidos exitosamente",
                    Data = empleados,
                    Count = totalCount
                };
            }
            catch (Exception ex)
            {
                return new PaginatedResponseDTO<IEnumerable<Empleado>>
                {
                    Status = false,
                    Message = $"Error al obtener los empleados: {ex.Message}",
                    Data = null,
                    Count = 0
                };
            }
        }

        // OBTENER POR ID:
        public async Task<ResponseDTO<Empleado>> GetEmpleadoByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new ResponseDTO<Empleado>
                    {
                        Status = false,
                        Message = "El ID debe ser mayor a 0",
                        Data = null
                    };
                }

                var empleado = await _empleadoRepository.GetEmpleadoByIdAsync(id);

                if (empleado == null)
                {
                    return new ResponseDTO<Empleado>
                    {
                        Status = false,
                        Message = "Empleado no encontrado",
                        Data = null
                    };
                }

                return new ResponseDTO<Empleado>
                {
                    Status = true,
                    Message = "Empleado obtenido exitosamente",
                    Data = empleado
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<Empleado>
                {
                    Status = false,
                    Message = $"Error al obtener el empleado: {ex.Message}",
                    Data = null
                };
            }
        }

        // ACTUALIZAR:
        public async Task<ResponseDTO<Empleado>> UpdateEmpleadoAsync(int id, Empleado empleado)
        {
            try
            {
                if (id <= 0)
                {
                    return new ResponseDTO<Empleado>
                    {
                        Status = false,
                        Message = "El ID debe ser mayor a 0",
                        Data = null
                    };
                }

                if (string.IsNullOrEmpty(empleado.Nombre))
                {
                    return new ResponseDTO<Empleado>
                    {
                        Status = false,
                        Message = "El nombre es requerido",
                        Data = null
                    };
                }

                if (string.IsNullOrEmpty(empleado.Puesto))
                {
                    return new ResponseDTO<Empleado>
                    {
                        Status = false,
                        Message = "El puesto es requerido",
                        Data = null
                    };
                }

                if (string.IsNullOrEmpty(empleado.Contraseña))
                {
                    return new ResponseDTO<Empleado>
                    {
                        Status = false,
                        Message = "La contraseña es requerida",
                        Data = null
                    };
                }

                // Asegurar que el ID del objeto coincida con el parámetro
                empleado.Id = id;

                var updatedEmpleado = await _empleadoRepository.UpdateEmpleadoAsync(empleado);

                if (updatedEmpleado == null)
                {
                    return new ResponseDTO<Empleado>
                    {
                        Status = false,
                        Message = "Empleado no encontrado",
                        Data = null
                    };
                }

                return new ResponseDTO<Empleado>
                {
                    Status = true,
                    Message = "Empleado actualizado exitosamente",
                    Data = updatedEmpleado
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<Empleado>
                {
                    Status = false,
                    Message = $"Error al actualizar el empleado: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}