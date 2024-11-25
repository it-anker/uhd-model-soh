using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Processes;

[DataContract]
public class ParameterLimitResponse<T>(
    List<T> select,
    int totalCount,
    int pageSize,
    int currentPage)
{
    [DataMember(Name = "data")] public List<T> Data { get; set; } = select;

    [DataMember(Name = "currentPage")] public int CurrentPage { get; set; } = currentPage;

    [DataMember(Name = "totalPages")] public int TotalPages { get; set; } = (int)Math.Ceiling(totalCount / (double)pageSize);

    [DataMember(Name = "totalCount")] public int TotalCount { get; set; } = totalCount;

    [DataMember(Name = "pageSize")] public int PageSize { get; set; } = pageSize;

    [DataMember(Name = "hasPreviousPage")] public bool HasPreviousPage => CurrentPage > 1;

    [DataMember(Name = "hasNextPage")] public bool HasNextPage => CurrentPage < TotalPages - 1;
}