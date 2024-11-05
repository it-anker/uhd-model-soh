using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Common;

public class CustomValidationResult
{
    private List<CustomValidationFailure> _errors = [];

    /// <summary>
    ///     Whether validation succeeded
    /// </summary>
    [IgnoreDataMember]
    public virtual bool IsValid => _errors.Count != 0;

    /// <summary>
    ///     A collection of errors.
    /// </summary>
    [DataMember(Name = "errors")]
    public IEnumerable<CustomValidationFailure> Errors
    {
        get => _errors;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _errors = value.Where(failure => failure != null!).ToList();
        }
    }

    /// <summary>
    ///     Generates a string representation of the error messages separated by new lines.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return ToString(Environment.NewLine);
    }

    /// <summary>
    ///     Generates a string representation of the error messages separated by the specified character.
    /// </summary>
    /// <param name="separator">The character to separate the error messages.</param>
    /// <returns></returns>
    public string ToString(string separator)
    {
        return string.Join(separator, _errors.Select(failure => failure.ErrorMessage));
    }
}