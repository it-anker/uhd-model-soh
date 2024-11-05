namespace SOH.Process.Server.Models.Common.Exceptions;

public class InternalServerException : CustomException
{
    public InternalServerException(string message, List<string>? errors = default)
        : base(message, errors)
    {
    }
}