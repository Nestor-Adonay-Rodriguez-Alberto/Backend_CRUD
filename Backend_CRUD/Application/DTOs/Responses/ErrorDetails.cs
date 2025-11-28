namespace Backend_CRUD.Domain.DTOs.Responses
{
    public class ErrorDetails
    {
        public string? Field { get; set; }
        public object? ProvidedValue { get; set; }
        public string? Conflict { get; set; }
    }
}
