using System.Net;

namespace SOH.Process.Server.Models.Common.Exceptions;

public class ConflictException(string message) : CustomException(message, null, HttpStatusCode.Conflict);