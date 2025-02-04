using System.ComponentModel.DataAnnotations;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Simulations;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

public class ProcessesController(
    ISimulationService simulationService,
    LinkGenerator linkGenerator,
    JsonSerializerSettings serializerSettings)
    : BaseApiRouteController
{
    /// <summary>
    ///     retrieve the list of available processes.
    /// </summary>
    /// <remarks>
    ///     The list of processes contains a summary of each process the OGC API - Processes offers,
    ///     including the link to a more detailed description of the process.
    ///     For more information, see [Section 7.9](https://docs.ogc.org/is/18-062/18-062.html#sc_process_list).
    /// </remarks>
    /// <response code="200">Information about the available processes.</response>
    [HttpGet]
    [ValidateModelState]
    [SwaggerOperation("GetProcesses")]
    [SwaggerResponse(200,
        type: typeof(ProcessList), description: "Information about the available processes")]
    public async Task<ActionResult<ProcessList>> GetProcessesAsync(
        [FromQuery(Name = "limit")] int? limit,
        [FromQuery(Name = "searchQuery")] string? query,
        CancellationToken token = default)
    {
        var request = new SearchProcessRequest { Query = query };
        if (limit.HasValue)
        {
            request.PageSize = limit.Value;
        }

        return Ok(await simulationService.ListProcessesAsync(request, token));
    }

    /// <summary>
    ///     Retrieves a process description by ID.
    /// </summary>
    /// <remarks>
    ///     The process description contains information about inputs and outputs and a link to the execution-endpoint for
    ///     the process. The Core does not mandate the use of a specific process description to specify the interface of a
    ///     process. That said, the Core requirements class makes the following recommendation:  Implementations SHOULD
    ///     consider supporting the OGC process description.  For more information, see [Section
    ///     7.10](https://docs.ogc.org/is/18-062/18-062.html#sc_process_description).
    /// </remarks>
    /// <response code="200">A process description.</response>
    /// <response code="404">The requested URI was not found.</response>
    [HttpGet("{processId}")]
    [ValidateModelState]
    [SwaggerOperation("GetProcessDescription")]
    [SwaggerResponse(200,
        type: typeof(ProcessDescription), description: "A process description.")]
    [SwaggerResponse(404,
        type: typeof(ProblemDetails), description: "The requested URI was not found.")]
    public async Task<ActionResult<ProcessDescription>> GetProcessDescriptionAsync(
        [FromRoute] [Required] string processId, CancellationToken token)
    {
        var process = await simulationService.GetSimulationAsync(processId, token);
        return Ok(process.Adapt<ProcessDescription>());
    }

    /// <summary>
    ///     execute a process.
    /// </summary>
    /// <remarks>
    ///     Create a new job.  For more information, see
    ///     [Section 7.11](https://docs.ogc.org/is/18-062/18-062.html#sc_create_job).
    /// </remarks>
    /// <param name="processId">The process to start.</param>
    /// <param name="request">Mandatory execute request JSON.</param>
    /// <param name="token">Cancellation request.</param>
    /// <response code="200">Result of synchronous execution.</response>
    /// <response code="201">Started asynchronous execution. Created job.</response>
    /// <response code="404">The requested URI was not found.</response>
    /// <response code="500">A server error occurred.</response>
    [HttpPost("{processId}/execution")]
    [ValidateModelState]
    [SwaggerOperation("Execute")]
    [SwaggerResponse(200, type: typeof(Result), description: "Result of synchronous execution")]
    [SwaggerResponse(200, type: typeof(MultipartContent),
        description: "The collection of results for multiple selected outputs.")]
    [SwaggerResponse(200, description: "For single output execution, as per output definition from process description")]
    [SwaggerResponse(201, type: typeof(StatusInfo), description: "Started asynchronous execution. Created job.")]
    [SwaggerResponse(204, description: "Successful operation (no response body)")]
    [SwaggerResponse(400, type: typeof(ProblemDetails), description: "The request is not valid.")]
    [SwaggerResponse(404, type: typeof(ProblemDetails), description: "The requested URI was not found.")]
    [SwaggerResponse(500, type: typeof(ProblemDetails), description: "A server error occurred.")]
    public async Task<IActionResult> ExecuteAsync(
        [FromRoute] [Required] string processId,
        [FromBody] Execute? request, CancellationToken token = default)
    {
        var create = new CreateSimulationJobRequest
        {
            SimulationId = processId, Execute = request ?? new Execute()
        };

        var response = await Mediator.Send(create, token);

        if (!string.IsNullOrEmpty(response.HangfireJobKey))
        {
            return StatusCode(201, response.Adapt<StatusInfo>());
        }

        ArgumentException.ThrowIfNullOrEmpty(response.ResultId);
        var resultResponse = await Mediator.Send(new GetJobResultRequest { JobId = response.JobId }, token);
        return this.GetActionResult(linkGenerator, HttpContext, resultResponse, serializerSettings);
    }
}