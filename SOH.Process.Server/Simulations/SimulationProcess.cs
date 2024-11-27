using System.Runtime.Serialization;
using MediatR;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Simulations;

public class SimulationProcess : Models.Processes.Process
{
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

public class CreateSimulationJobRequest : IRequest<SimulationJob>
{
    /// <summary>
    ///     The reference ID to the running job of this process.
    /// </summary>
    [DataMember(Name = "processID")]
    public string SimulationId { get; set; } = default!;

    /// <summary>
    ///     The details of the execution with inputs and output configuration.
    /// </summary>
    [DataMember(Name = "execute")]
    public Execute Execute { get; set; } = default!;
}