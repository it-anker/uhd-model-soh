using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
///     Optional URIs for callbacks for this job.  Support for this parameter is not required and the parameter may be
///     removed from the API definition, if conformance class **&#x27;callback&#x27;** is not listed in the conformance
///     declaration under &#x60;/conformance&#x60;.
/// </summary>
[DataContract]
public sealed class Subscriber : IEquatable<Subscriber>
{
    /// <summary>
    ///     Gets or Sets SuccessUri.
    /// </summary>
    [DataMember(Name = "successUri")]
    public string? SuccessUri { get; set; }

    /// <summary>
    ///     Gets or Sets InProgressUri.
    /// </summary>
    [DataMember(Name = "inProgressUri")]
    public string? InProgressUri { get; set; }

    /// <summary>
    ///     Gets or Sets FailedUri.
    /// </summary>
    [DataMember(Name = "failedUri")]
    public string? FailedUri { get; set; }

    public bool Equals(Subscriber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return SuccessUri == other.SuccessUri &&
               InProgressUri == other.InProgressUri &&
               FailedUri == other.FailedUri;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Subscriber)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SuccessUri, InProgressUri, FailedUri);
    }
}