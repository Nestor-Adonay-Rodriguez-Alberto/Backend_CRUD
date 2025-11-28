using System;


namespace Backend_CRUD.Domain.Entities
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Puesto { get; set; }
        public string Contraseña { get; set; }
    }
}
