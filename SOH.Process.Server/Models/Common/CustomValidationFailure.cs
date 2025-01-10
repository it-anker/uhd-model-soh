using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FluentValidation;
using Newtonsoft.Json.Converters;

namespace SOH.Process.Server.Models.Common;

/// <summary>
///     The validation severities returned by the validator.
/// </summary>
[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
public enum CustomSeverity
{
    [EnumMember(Value = "error")] Error,

    [EnumMember(Value = "warning")] Warning,

    [EnumMember(Value = "info")] Info
}

/// <summary>
///     Defines a validation failure.
/// </summary>
[Serializable]
public class CustomValidationFailure : IEqualityComparer<CustomValidationFailure>
{
    /// <summary>
    ///     The name of the property.
    /// </summary>
    [Required]
    [DataMember(Name = "propertyName")]
    public string PropertyName { get; set; } = null!;

    /// <summary>
    ///     The concrete message of an error, warning or info.
    /// </summary>
    [Required]
    [DataMember(Name = "errorMessage")]
    public string ErrorMessage { get; set; } = null!;

    /// <summary>
    ///     Custom severity level associated with the failure.
    /// </summary>
    [DataMember(Name = "severity")]
    public CustomSeverity Severity { get; set; } = CustomSeverity.Error;

    /// <summary>
    ///     Gets or sets the unique identifiable error code to react on it.
    /// </summary>
    [DataMember(Name = "errorCode")]
    public string? ErrorCode { get; set; }

    /// <summary>
    ///     Creates a textual representation of the failure.
    /// </summary>
    public override string ToString()
    {
        return ErrorMessage;
    }

    public bool Equals(CustomValidationFailure? x, CustomValidationFailure? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        return x.PropertyName == y.PropertyName
               && x.Severity == y.Severity
               && x.ErrorMessage == y.ErrorMessage
               && x.ErrorCode == y.ErrorCode;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is CustomValidationFailure failure) return Equals(this, failure);

        return false;
    }

    public int GetHashCode(CustomValidationFailure obj)
    {
        return HashCode.Combine(obj.PropertyName, obj.ErrorMessage, (int)obj.Severity, obj.ErrorCode);
    }

    public override int GetHashCode()
    {
        return GetHashCode(this);
    }
}