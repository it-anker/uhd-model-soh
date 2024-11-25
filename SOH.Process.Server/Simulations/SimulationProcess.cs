using System.Runtime.Serialization;
using MediatR;

namespace SOH.Process.Server.Simulations;

public class SimulationProcess : Models.Processes.Process
{
    /// <summary>
    ///     The reference ID to the running job of this process.
    /// </summary>
    [DataMember(Name = "jobId")]
    public string? JobId { get; set; }

    /// <summary>
    ///     The reference ID to the result of this process.
    /// </summary>
    [DataMember(Name = "resultId")]
    public string? ResultId { get; set; }

    /// <summary>
    ///     The time in UTC when this job was created.
    /// </summary>
    [DataMember(Name = "created")]
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     The last modification time in UTC of this job.
    /// </summary>
    [DataMember(Name = "updated")]
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

    public void Update(SimulationProcess request)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Title);
        ArgumentException.ThrowIfNullOrEmpty(request.Version);

        JobId = request.JobId;
        ResultId = request.ResultId;

        Title = request.Title?.Trim();
        Version = request.Version.Trim();
        Keywords = request.Keywords.Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
        Description = request.Description?.Trim();
        OutputTransmission = request.OutputTransmission;
        Inputs = request.Inputs;
        Outputs = request.Outputs;
        Metadata = request.Metadata;
        AdditionalParameters = request.AdditionalParameters;
        JobControlOptions = request.JobControlOptions;
    }
}

public class UpdateSimulationProcessRequest : SimulationProcess, IRequest<SimulationProcess>;

public class CreateSimulationProcessRequest : SimulationProcess, IRequest<SimulationProcess>;

public class CreateSimulationJobRequest : SimulationJob, IRequest<SimulationJob>;