namespace Backend_CRUD.Domain.DTOs.Responses
{
    public class ResponseDTO<T>
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public T? Data { get; set; }
    }
}
