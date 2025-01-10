using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public class Schema
{
    [DataMember(Name = "title", EmitDefaultValue = false)]
    public string? Title { get; set; }

    [DataMember(Name = "multipleOf", EmitDefaultValue = false), Range(0, double.PositiveInfinity)]
    public double? MultipleOf { get; set; }

    [DataMember(Name = "maximum", EmitDefaultValue = false)]
    public double? Maximum { get; set; }

    [DataMember(Name = "exclusiveMaximum", EmitDefaultValue = false)]
    public bool? ExclusiveMaximum { get; set; }

    [DataMember(Name = "minimum", EmitDefaultValue = false)]
    public double? Minimum { get; set; }

    [DataMember(Name = "exclusiveMinimum", EmitDefaultValue = false)]
    public bool? ExclusiveMinimum { get; set; }

    [DataMember(Name = "maxLength", EmitDefaultValue = false), Range(0, int.MaxValue)]
    public int? MaxLength { get; set; }

    [DataMember(Name = "minLength", EmitDefaultValue = false), Range(0, int.MaxValue)]
    public int MinLength { get; set; }

    [DataMember(Name = "pattern", EmitDefaultValue = false)]
    public string? Pattern { get; set; }

    [DataMember(Name = "maxItems", EmitDefaultValue = false), Range(0, int.MaxValue)]
    public int? MaxItems { get; set; }

    [DataMember(Name = "minItems", EmitDefaultValue = false), Range(0, int.MaxValue)]
    public int? MinItems { get; set; }

    [DataMember(Name = "uniqueItems", EmitDefaultValue = false)]
    public bool UniqueItems { get; set; }

    [DataMember(Name = "maxProperties", EmitDefaultValue = false)]
    public int? MaxProperties { get; set; }

    [DataMember(Name = "minProperties", EmitDefaultValue = false)]
    public int? MinProperties { get; set; }

    [DataMember(Name = "type", EmitDefaultValue = false)]
    public string? Type { get; set; }

    [DataMember(Name = "not", EmitDefaultValue = false)]
    public Schema? Not { get; set; }

    [DataMember(Name = "description", EmitDefaultValue = false)]
    public string? Description { get; set; }

    [DataMember(Name = "format", EmitDefaultValue = false)]
    public string? Format { get; set; }

    [DataMember(Name = "default", EmitDefaultValue = false)]
    public object? Default { get; set; }

    [DataMember(Name = "nullable", EmitDefaultValue = false)]
    public bool Nullable { get; set; }

    [DataMember(Name = "readOnly", EmitDefaultValue = false)]
    public bool ReadOnly { get; set; }

    [DataMember(Name = "writeOnly", EmitDefaultValue = false)]
    public bool WriteOnly { get; set; }

    [DataMember(Name = "example", EmitDefaultValue = false)]
    public object? Example { get; set; }

    [DataMember(Name = "deprecated", EmitDefaultValue = false)]
    public bool? Deprecated { get; set; }

    [DataMember(Name = "contentMediaType", EmitDefaultValue = false)]
    public string? ContentMediaType { get; set; }

    [DataMember(Name = "contentEncoding", EmitDefaultValue = false)]
    public string? ContentEncoding { get; set; }

    [DataMember(Name = "contentSchema", EmitDefaultValue = false)]
    public string? ContentSchema { get; set; }

    [DataMember(Name = "required", EmitDefaultValue = false)]
    public List<string> Required { get; set; } = [];

    [DataMember(Name = "enum", EmitDefaultValue = false)]
    public List<object> Enum { get; set; } = [];

    [DataMember(Name = "allOf", EmitDefaultValue = false)]
    public List<Schema> AllOf { get; set; } = [];

    [DataMember(Name = "oneOf", EmitDefaultValue = false)]
    public List<Schema> OneOf { get; set; } = [];

    [DataMember(Name = "anyOf", EmitDefaultValue = false)]
    public List<Schema> AnyOf { get; set; } = [];

    [DataMember(Name = "items", EmitDefaultValue = false)]
    public object? Items { get; set; }

    [DataMember(Name = "properties", EmitDefaultValue = false)]
    public Dictionary<string, Schema> Properties { get; set; } = [];

    public bool ShouldSerializeProperties() => Properties != null! && Properties.Count > 0;

    public bool ShouldSerializeAnyOf() => AnyOf != null! && AnyOf.Count > 0;

    public bool ShouldSerializeAllOf() => AllOf != null! && AllOf.Count > 0;
    public bool ShouldSerializeOneOf() => OneOf != null! && OneOf.Count > 0;
    public bool ShouldSerializeEnum() => Enum != null! && Enum.Count > 0;
    public bool ShouldSerializeRequired() => Required != null! && Required.Count > 0;
}

[DataContract]
public class Reference
{
    [DataMember(Name = "$ref", EmitDefaultValue = false)]
    public string RefId { get; set; }
}