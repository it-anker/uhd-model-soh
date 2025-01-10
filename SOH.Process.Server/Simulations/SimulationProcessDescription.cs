using System.Runtime.Serialization;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Simulations;

public class SimulationProcessDescription : Models.Processes.ProcessDescription
{
    /// <summary>
    ///     The reference name to concrete point on the entrypoint or name.
    /// </summary>
    [DataMember(Name = "executionKind")]
    public ProcessExecutionKind ExecutionKind { get; set; }

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

    [DataMember(Name = "isTest")]
    public bool IsTest { get; set; }

    public void Update(SimulationProcessDescription request)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Title);
        ArgumentException.ThrowIfNullOrEmpty(request.Version);

        Title = request.Title?.Trim();
        Version = request.Version.Trim();
        Keywords = request.Keywords.Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
        Description = request.Description?.Trim();
        Inputs = request.Inputs;
        IsTest = request.IsTest;
        Outputs = request.Outputs;
        Metadata = request.Metadata;
        AdditionalParameters = request.AdditionalParameters;
        JobControlOptions = request.JobControlOptions.Distinct().ToList();
        OutputTransmission = request.OutputTransmission.Distinct().ToList();
    }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum ProcessExecutionKind
{
    [EnumMember(Value = "direct")] Direct
}

public class UpdateSimulationProcessDescriptionRequest : SimulationProcessDescription, IRequest<SimulationProcessDescription>;

public class CreateSimulationProcessDescriptionRequest : SimulationProcessDescription, IRequest<SimulationProcessDescription>;

public class CreateSimulationJobRequest : IRequest<SimulationJob>
{
    /// <summary>
    ///     The reference ID to the running job of this process.
    /// </summary>
    [DataMember(Name = "processID")]
    public string SimulationId { get; set; } = null!;

    /// <summary>
    ///     The details of the execution with inputs and output configuration.
    /// </summary>
    [DataMember(Name = "execute")]
    public Execute Execute { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the preferred option.
    /// </summary>
    [DataMember(Name = "execute")]
    public JobControlOptions? Prefer { get; set; }
}