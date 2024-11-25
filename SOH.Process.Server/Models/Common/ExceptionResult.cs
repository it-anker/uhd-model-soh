using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Common;

/// <summary>
///     JSON schema for exceptions based on RFC 7807.
/// </summary>
[DataContract]
public sealed class ExceptionResult : Dictionary<string, object>
{
    [DataMember(Name = "errorId")]
    [Required]
    public string ErrorId { get; set; } = default!;

    [DataMember(Name = "source")]
    public string? Source { get; set; }

    [Required]
    [DataMember(Name = "statusCode")]
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;

    [DataMember(Name = "stackTrace")]
    public string? StackTrace { get; set; }

    [DataMember(Name = "exception")]
    [Required]
    public string Exception { get; set; } = default!;

    [DataMember(Name = "messages")]
    [Required]
    public List<string> Messages { get; set; } = [];

    [DataMember(Name = "supportMessage")]
    public string? SupportMessage { get; set; }
}