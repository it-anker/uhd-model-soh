using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
///     Optional URIs for callbacks for this job.  Support for this parameter is not required and the parameter may be
///     removed from the API definition, if conformance class **&#x27;callback&#x27;** is not listed in the conformance
///     declaration under &#x60;/conformance&#x60;.
/// </summary>
[DataContract]
public class Subscriber
{
    /// <summary>
    ///     Callback uri when process is finished.
    /// </summary>
    [DataMember(Name = "successUri")]
    public string? SuccessUri { get; set; }

    /// <summary>
    ///     Callback uri when starting process.
    /// </summary>
    [DataMember(Name = "inProgressUri")]
    public string? InProgressUri { get; set; }

    /// <summary>
    ///     Callback uri when process failed.
    /// </summary>
    [DataMember(Name = "failedUri")]
    public string? FailedUri { get; set; }
}