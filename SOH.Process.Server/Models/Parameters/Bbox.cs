using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Models.Parameters;

[DataContract]
public sealed class Bbox
{
    /// <summary>
    ///     Gets or Sets _Bbox.
    /// </summary>
    [Required]
    [DataMember(Name = "bbox")]
    public List<decimal?> BoundingBox { get; set; } = [];

    /// <summary>
    ///     Gets or Sets Crs.
    /// </summary>
    [DataMember(Name = "crs")]
    public CoordinateReferenceSystem? CoordinateReferenceSystem { get; set; }

    public bool ShouldSerializeBoundingBox() => BoundingBox != null! && BoundingBox.Count > 0 &&
                                                BoundingBox.All(arg => arg.HasValue);
}