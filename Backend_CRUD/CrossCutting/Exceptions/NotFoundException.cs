namespace Backend_CRUD.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public int StatusCode { get; }

        public NotFoundException() : base("Bad request error.")
        {
            StatusCode = 404;
        }

        public NotFoundException(string message) : base(message)
        {
            StatusCode = 404;
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = 404;
        }

        public NotFoundException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
