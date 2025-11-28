using Backend_CRUD.Domain.Entities;
using Backend_CRUD.Domain.Interfaces.Repositories;
using Backend_CRUD.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Backend_CRUD.Infrastructure.Persistence.Repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly AppDbContext _context;

        public EmpleadoRepository(AppDbContext context)
        {
            _context = context;
        }





        // CREAR:
        public async Task<Empleado> CreateEmpleadoAsync(Empleado empleado)
        {
            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();
            return empleado;
        }


        // OBTENER LISTADO:
        public async Task<(IEnumerable<Empleado> Empleados, int TotalCount)> GetEmpleadosAsync(int page, int pageSize, string? search)
        {
            var query = _context.Empleados.AsQueryable();

            // Aplicar filtro de búsqueda si se proporciona (buscar en nombre o puesto)
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Nombre.Contains(search) || e.Puesto.Contains(search));
            }

            // Obtener el total de registros antes de la paginación
            var totalCount = await query.CountAsync();

            // Aplicar paginación
            var empleados = await query
                .OrderBy(e => e.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (empleados, totalCount);
        }


        // OBTENER POR ID:
        public async Task<Empleado?> GetEmpleadoByIdAsync(int id)
        {
            return await _context.Empleados.FindAsync(id);
        }


        // ACTUALIZAR:
        public async Task<Empleado?> UpdateEmpleadoAsync(Empleado empleado)
        {
            var existingEmpleado = await _context.Empleados.FindAsync(empleado.Id);
            
            if (existingEmpleado == null)
                return null;

            existingEmpleado.Nombre = empleado.Nombre;
            existingEmpleado.Puesto = empleado.Puesto;
            existingEmpleado.Contraseña = empleado.Contraseña;

            await _context.SaveChangesAsync();
            return existingEmpleado;
        }
  
    }
}