namespace Backend_CRUD.Domain.DTOs.Responses
{
    public class ErrorResponseDTO<T>
    {
        public string? ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public T? ErrorBody { get; set; }
        public ErrorDetails? ErrorDetails { get; set; }
    }
}
