using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Common;

/// <summary>
///     JSON schema for exceptions based on RFC 7807.
/// </summary>
[DataContract]
public sealed class ExceptionResult : Dictionary<string, object>, IEquatable<ExceptionResult>
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

    public bool Equals(ExceptionResult? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ErrorId == other.ErrorId;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is ExceptionResult other && Equals(other));
    }

    public override int GetHashCode()
    {
        return ErrorId.GetHashCode();
    }
}