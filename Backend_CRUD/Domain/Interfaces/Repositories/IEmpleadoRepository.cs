using Backend_CRUD.Domain.Entities;

namespace Backend_CRUD.Domain.Interfaces.Repositories
{
    public interface IEmpleadoRepository
    {
        Task<Empleado> CreateEmpleadoAsync(Empleado empleado);
        Task<(IEnumerable<Empleado> Empleados, int TotalCount)> GetEmpleadosAsync(int page, int pageSize, string? search);
        Task<Empleado?> GetEmpleadoByIdAsync(int id);
        Task<Empleado?> UpdateEmpleadoAsync(Empleado empleado);
        Task<Empleado?> GetEmpleadoByCredentialsAsync(string nombre, string contraseña);
    }
}