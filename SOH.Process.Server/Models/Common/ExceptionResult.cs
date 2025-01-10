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
    public string ErrorId { get; set; } = null!;

    [Required, DataMember(Name = "type")]
    public string Type { get; set; } = null!;

    [DataMember(Name = "title")]
    public string? Title { get; set; }

    [Required]
    [DataMember(Name = "statusCode")]
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;

    [DataMember(Name = "details")]
    public string? Exception { get; set; }
}