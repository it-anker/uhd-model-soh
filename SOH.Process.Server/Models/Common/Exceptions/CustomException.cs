using System.Net;

namespace SOH.Process.Server.Models.Common.Exceptions;

public class CustomException : Exception
{
    protected CustomException(string message, List<string>? errors = null,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message)
    {
        ErrorMessages = errors;
        StatusCode = statusCode;
    }

    public List<string>? ErrorMessages { get; }

    public HttpStatusCode StatusCode { get; }
}