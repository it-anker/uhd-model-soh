using System.Runtime.Serialization;
using SOH.Process.Server.Models.Processes;

namespace SOH.Process.Server.Simulations;

public class SimulationJob : StatusInfo
{
    [DataMember(Name = "jobStatusKey")]
    public string? HangfireJobKey { get; set; }

    [DataMember(Name = "resultId")]
    public string? ResultId { get; set; }

    [DataMember(Name = "cancelRequested")]
    public bool IsCancellationRequested { get; set; }

    [DataMember(Name = "failedMessage")]
    public string? ExceptionMessage { get; set; }

    public void Update(SimulationJob job)
    {
        ResultId = job.ResultId;
        Message = job.Message?.Trim();
        UpdatedUtc = DateTime.UtcNow;
        StartedUtc = job.StartedUtc;
        FinishedUtc = job.FinishedUtc;
        Progress = job.Progress;
        Type = job.Type;
        Status = job.Status;

        Links = job.Links
            .Where(link => !string.IsNullOrEmpty(link.Href))
            .Select(link => link.Trim())
            .ToList();
    }
}