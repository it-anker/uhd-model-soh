using System.Net;

namespace SOH.Process.Server.Models.Common.Exceptions;

public class BadRequestException(
    string message,
    List<string>? errors = null,
    HttpStatusCode statusCode = HttpStatusCode.BadRequest) : CustomException(message, errors, statusCode);