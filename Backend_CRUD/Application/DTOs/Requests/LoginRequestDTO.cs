namespace Backend_CRUD.Application.DTOs.Requests
{
    public class LoginRequestDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
    }
}