using System.Reflection;
using System.Runtime.Serialization;

namespace Backend_CRUD.Domain.Exceptions
{
    public class BadRequestException<T> : Exception
    {
        public int StatusCode { get; }
        public T? ExceptionBody { get; }
        public string? ErrorType { get; }

        public BadRequestException() : base("Bad request error.")
        {
            StatusCode = 400;
        }

        public BadRequestException(string message) : base(message)
        {
            StatusCode = 400;
        }

        public BadRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = 400;
        }

        public BadRequestException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public BadRequestException(string message, T? exceptionBody) : base(message)
        {
            StatusCode = 400;
            ExceptionBody = exceptionBody;
        }
        public BadRequestException(string message, T? exceptionBody, Enum errorType) : base(message)
        {
            StatusCode = 400;
            ExceptionBody = exceptionBody;
            ErrorType = errorType.GetEnumMemberValue();
        }
    }

    public static class EnumExtensions
    {
        public static string GetEnumMemberValue(this Enum enumValue)
        {
            var type = enumValue.GetType();
            var member = type.GetMember(enumValue.ToString()).FirstOrDefault();
            var attribute = member?.GetCustomAttribute<EnumMemberAttribute>();

            return attribute?.Value ?? enumValue.ToString();
        }
    }
}
