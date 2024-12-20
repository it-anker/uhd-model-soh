using System.ComponentModel.DataAnnotations;
using Mapster;
using Mars.Common.Core.Collections;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Simulations;
using Swashbuckle.AspNetCore.Annotations;
using Results = SOH.Process.Server.Models.Ogc.Results;

namespace SOH.Process.Server.Controllers.V1;

public class JobsController(
    ISimulationService simulationService,
    LinkGenerator linkGenerator,
    JsonSerializerSettings serializerSettings) : BaseApiRouteController
{
    /// <summary>
    ///     retrieve the list of jobs.
    /// </summary>
    /// <remarks>
    ///     Lists available jobs.  For more information, see
    ///     [Section 11](https://docs.ogc.org/is/18-062/18-062.html#sc_job_list).
    /// </remarks>
    /// <response code="200">A list of jobs for this process.</response>
    /// <response code="404">The requested URI was not found.</response>
    [HttpGet]
    [ValidateModelState]
    [SwaggerOperation("GetJobs")]
    [SwaggerResponse(200, type: typeof(JobList), description: "A list of jobs for this process.")]
    [SwaggerResponse(404, type: typeof(ProblemDetails), description: "The requested URI was not found.")]
    public async Task<ActionResult<JobList>> GetJobsAsync(
        [FromQuery(Name = "limit")] int? limit,
        [FromQuery(Name = "page")] int? page,
        [FromQuery(Name = "searchQuery")] string? query,
        CancellationToken token = default)
    {
        var request = new SearchJobProcessRequest { Query = query };
        if (limit.HasValue) request.PageSize = limit.GetValueOrDefault();
        if (page.HasValue) request.PageNumber = page.GetValueOrDefault();
        return Ok(await simulationService.ListJobsAsync(request, token));
    }

    /// <summary>
    ///     retrieve the status of a job.
    /// </summary>
    /// <remarks>
    ///     Shows the status of a job.   For more information, see [Section
    ///     7.12](https://docs.ogc.org/is/18-062/18-062.html#sc_retrieve_status_info).
    /// </remarks>
    /// <param name="jobId">local identifier of a job.</param>
    /// <response code="200">The status of a job.</response>
    /// <response code="404">The requested URI was not found.</response>
    /// <response code="500">A server error occurred.</response>
    [HttpGet("{jobId}")]
    [ValidateModelState]
    [SwaggerOperation("GetStatus")]
    [SwaggerResponse(200, type: typeof(StatusInfo), description: "The status of a job.")]
    [SwaggerResponse(404, type: typeof(ProblemDetails), description: "The requested URI was not found.")]
    [SwaggerResponse(500, type: typeof(ProblemDetails), description: "A server error occurred.")]
    public virtual async Task<ActionResult<StatusInfo>> GetStatusAsync([FromRoute] [Required] string jobId)
    {
        var status = (await simulationService.GetSimulationJobAsync(jobId)).Adapt<StatusInfo>();
        return Ok(status);
    }

    /// <summary>
    ///     retrieve the result(s) of a job.
    /// </summary>
    /// <remarks>
    ///     Lists available results of a job. In case of a failure, lists exceptions instead.  For more information, see
    ///     [Section 7.13](https://docs.ogc.org/is/18-062/18-062.html#sc_retrieve_job_results).
    /// </remarks>
    /// <param name="jobId">local identifier of a job.</param>
    /// <param name="token">cancellation token.</param>
    /// <response code="200">The results of a job.</response>
    /// <response code="404">The requested URI was not found.</response>
    /// <response code="500">A server error occurred.</response>
    [HttpGet("{jobId}/results")]
    [ValidateModelState]
    [SwaggerOperation("GetResult")]
    [Produces("application/json", "multipart/related")]
    [SwaggerResponse(200, type: typeof(Results), description: "The document results of a job.")]
    [SwaggerResponse(200, type: typeof(MultipartContent),
        description: "The collection of results for multiple selected outputs.")]
    [SwaggerResponse(204, description: "The results with links to the references.")]
    [SwaggerResponse(404, type: typeof(ProblemDetails), description: "The requested URI was not found.")]
    [SwaggerResponse(500, type: typeof(ProblemDetails), description: "A server error occurred.")]
    public async Task<IActionResult> GetResultAsync([FromRoute] [Required] string jobId, CancellationToken token = default)
    {
        var response = await Mediator.Send(new GetJobResultRequest { JobId = jobId }, token);
        return this.GetActionResult(linkGenerator, HttpContext, response, serializerSettings);
    }

    /// <summary>
    ///     cancel a job execution, remove a finished job.
    /// </summary>
    /// <remarks>
    ///     Cancel a job execution and remove it from the jobs list.  For more information, see [Section
    ///     13](https://docs.ogc.org/is/18-062/18-062.html#Dismiss).
    /// </remarks>
    /// <param name="jobId">local identifier of a job.</param>
    /// <param name="token">Cancellation request.</param>
    /// <response code="200">The status of a job.</response>
    /// <response code="404">The requested URI was not found.</response>
    /// <response code="500">A server error occurred.</response>
    [HttpDelete("{jobId}")]
    [ValidateModelState]
    [SwaggerOperation("Dismiss")]
    [SwaggerResponse(200, type: typeof(StatusInfo), description: "The status of a job.")]
    [SwaggerResponse(404, type: typeof(ProblemDetails), description: "The requested URI was not found.")]
    [SwaggerResponse(500, type: typeof(ProblemDetails), description: "A server error occurred.")]
    public virtual async Task<IActionResult> DismissAsync([FromRoute] [Required] string jobId, CancellationToken token)
    {
        return Ok((await simulationService.CancelJobAsync(jobId, token)).Adapt<StatusInfo>());
    }
}