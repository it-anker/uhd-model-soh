using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Processes;

[DataContract]
public class ParameterLimit
{
    private int _pageSize = 10;

    [DataMember(Name = "pageNumber")]
    public int PageNumber { get; set; }

    [DataMember(Name = "pageSize")]
    public int PageSize
    {
        get => _pageSize;
        set
        {
            _pageSize = value switch
            {
                > 10000 => 10000,
                <= 0 => 1,
                _ => value
            };
        }
    }
}