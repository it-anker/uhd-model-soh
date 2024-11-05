using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SOH.Process.Server.Models.Ogc;

namespace IO.Swagger.Models;

[DataContract]
public partial class Reference : IEquatable<Reference>
{
    /// <summary>
    /// Gets or Sets _Ref.
    /// </summary>
    [Required]
    [DataMember(Name = "$ref")]
    public string _Ref { get; set; } = default!;

    public bool Equals(Reference? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _Ref == other._Ref;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Reference)obj);
    }

    public override int GetHashCode()
    {
        return _Ref.GetHashCode();
    }
}