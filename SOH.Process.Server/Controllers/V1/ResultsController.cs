using Microsoft.AspNetCore.Mvc;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Simulations;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

public class ResultsController(
    ISimulationService simulationService,
    IResultService resultService) : BaseApiRouteController
{
    /// <summary>
    ///     retrieve result by id.
    /// </summary>
    /// <remarks>
    ///     An available result of a job.
    /// </remarks>
    /// <response code="200">The raw result data.</response>
    /// <response code="404">The requested URI was not found.</response>
    [HttpGet]
    [ValidateModelState]
    [SwaggerOperation("GetResult")]
    [SwaggerResponse(200, type: typeof(Result),
        description: "The result object associated with this.")]
    [SwaggerResponse(404, type: typeof(ProblemDetails),
        description: "The requested URI was not found.")]
    [SwaggerResponse(500, type: typeof(ProblemDetails),
        description: "A server error occurred.")]
    public async Task<Result> GetResultAsync(string resultId, CancellationToken token)
    {
        return await resultService.GetAsync(resultId, token);
    }

    /// <summary>
    ///     retrieve result by job id.
    /// </summary>
    /// <remarks>
    ///     An available result of a job.
    /// </remarks>
    /// <response code="200">The raw result data.</response>
    /// <response code="404">The requested URI was not found.</response>
    [HttpGet("{jobId}/job")]
    [ValidateModelState]
    [SwaggerOperation("GetResultByJobId")]
    [SwaggerResponse(200, type: typeof(Result),
        description: "The result object associated with this.")]
    [SwaggerResponse(404, type: typeof(ProblemDetails),
        description: "The requested URI was not found.")]
    [SwaggerResponse(500, type: typeof(ProblemDetails),
        description: "A server error occurred.")]
    public async Task<Result> GetResultByJobAsync(string jobId, CancellationToken token)
    {
        var job = await simulationService.GetSimulationJobAsync(jobId, token);
        return await resultService.GetAsync(job.ResultId ?? string.Empty, token);
    }
}