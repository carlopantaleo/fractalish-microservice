using System.Net;

namespace FractalishMicroservice.Abstractions.Exceptions;

public class ServiceInstanceException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ServiceInstanceException(HttpStatusCode statusCode, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
