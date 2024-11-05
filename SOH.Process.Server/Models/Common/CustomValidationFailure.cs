using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using FluentValidation;

namespace SOH.Process.Server.Models.Common;

/// <summary>
///     Defines a validation failure
/// </summary>
[Serializable]
public class CustomValidationFailure : IEqualityComparer<CustomValidationFailure>
{
    /// <summary>
    ///     The name of the property.
    /// </summary>
    [Required]
    [DataMember(Name = "propertyName")]
    public string PropertyName { get; set; } = default!;

    /// <summary>
    ///     The error message.
    /// </summary>
    [Required]
    [DataMember(Name = "errorMessage")]
    public string ErrorMessage { get; set; } = default!;

    /// <summary>
    ///     The property value that caused the failure.
    /// </summary>
    [DataMember(Name = "attemptedValue")]
    public object? AttemptedValue { get; set; }

    /// <summary>
    ///     Custom severity level associated with the failure.
    /// </summary>
    [DataMember(Name = "severity")]
    public Severity Severity { get; set; } = Severity.Error;

    /// <summary>
    ///     Gets or sets the unique identifiable error code to react on it
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