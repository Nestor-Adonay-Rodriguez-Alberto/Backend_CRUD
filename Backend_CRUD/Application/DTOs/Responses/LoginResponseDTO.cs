namespace Backend_CRUD.Application.DTOs.Responses
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Puesto { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}