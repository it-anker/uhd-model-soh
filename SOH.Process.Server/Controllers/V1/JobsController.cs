using System.ComponentModel.DataAnnotations;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Common;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Simulations;
using Swashbuckle.AspNetCore.Annotations;
using Results = SOH.Process.Server.Models.Ogc.Results;

namespace SOH.Process.Server.Controllers.V1;

[ApiController]
[Route("[controller]")]
public class JobsController(
    ISimulationService simulationService,
    IResultService resultService) : BaseApiRouteController
{
    /// <summary>
    ///     retrieve the list of jobs.
    /// </summary>
    /// <remarks>
    ///     Lists available jobs.  For more information, see [Section
    ///     11](https://docs.ogc.org/is/18-062/18-062.html#sc_job_list).
    /// </remarks>
    /// <response code="200">A list of jobs for this process.</response>
    /// <response code="404">The requested URI was not found.</response>
    [HttpGet]
    [ValidateModelState]
    [SwaggerOperation("GetJobs")]
    [SwaggerResponse(200, type: typeof(JobList), description: "A list of jobs for this process.")]
    [SwaggerResponse(404, type: typeof(ProblemDetails), description: "The requested URI was not found.")]
    public async Task<ActionResult<JobList>> GetJobs(CancellationToken token = default)
    {
        return Ok(await simulationService.ListJobsAsync(token));
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
    public virtual async Task<ActionResult<StatusInfo>> GetStatus([FromRoute] [Required] string jobId)
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
    [SwaggerResponse(200, type: typeof(Results), description: "The results of a job.")]
    [SwaggerResponse(404, type: typeof(ProblemDetails), description: "The requested URI was not found.")]
    [SwaggerResponse(500, type: typeof(ProblemDetails), description: "A server error occurred.")]
    public async Task<ActionResult<Results>> GetResult([FromRoute] [Required] string jobId,
        CancellationToken token = default)
    {
        var job = await simulationService.GetSimulationJobAsync(jobId, token);

        var results = new Results();
        if (!string.IsNullOrEmpty(job.ResultId))
        {
            var result = await resultService.GetAsync(job.ResultId, token);
            results.Add(result.Id, result);
        }

        return Ok(results);
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
    public virtual async Task<IActionResult> Dismiss([FromRoute] [Required] string jobId, CancellationToken token)
    {
        return Ok((await simulationService.CancelJobAsync(jobId, token)).Adapt<StatusInfo>());
    }
}