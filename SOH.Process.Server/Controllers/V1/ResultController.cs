using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Common;
using SOH.Process.Server.Simulations;
using Swashbuckle.AspNetCore.Annotations;
using Results = Microsoft.AspNetCore.Http.Results;

namespace SOH.Process.Server.Controllers.V1;

public class ResultController(
    IResultService resultService,
    ISimulationService simulationService) : BaseApiController
{
    /// <summary>
    ///     retrieve the result(s) of a job.
    /// </summary>
    /// <remarks>
    ///     Lists available results of a job. In case of a failure, lists exceptions instead.  For more information, see
    ///     [Section 7.13](https://docs.ogc.org/is/18-062/18-062.html#sc_retrieve_job_results).
    /// </remarks>
    /// <param name="jobId">local identifier of a job</param>
    /// <param name="token">cancellation token</param>
    /// <response code="200">The results of a job.</response>
    /// <response code="404">The requested URI was not found.</response>
    /// <response code="500">A server error occurred.</response>
    [HttpGet]
    [Route("/jobs/{jobId}/results")]
    [ValidateModelState]
    [SwaggerOperation("GetResult")]
    [SwaggerResponse(200, type: typeof(Results), description: "The results of a job.")]
    [SwaggerResponse(404, type: typeof(ExceptionResult), description: "The requested URI was not found.")]
    [SwaggerResponse(500, type: typeof(ExceptionResult), description: "A server error occurred.")]
    public async Task<ActionResult<Models.Ogc.Results>> GetResult(
        [FromRoute] [Required] string jobId, CancellationToken token = default)
    {
        var job = await simulationService.GetSimulationJobAsync(jobId, token);
        var simulation = await simulationService.GetSimulationAsync(job.SimulationId, token);
        ArgumentNullException.ThrowIfNull(simulation.ResultId);

        var result = await resultService.GetAsync(simulation.ResultId, token);

        return Ok(new Models.Ogc.Results
        {
            { simulation.ResultId, result },
        });
    }
}