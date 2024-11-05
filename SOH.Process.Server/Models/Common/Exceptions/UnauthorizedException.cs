using System.Net;

namespace SOH.Process.Server.Models.Common.Exceptions;

public class UnauthorizedException(string message) : CustomException(message, null, HttpStatusCode.Unauthorized);