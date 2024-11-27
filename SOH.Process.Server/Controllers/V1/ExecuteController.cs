using System.ComponentModel.DataAnnotations;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Common;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Simulations;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

[ApiController]
public class ExecuteController(IResultService resultService) : BaseApiController
{
    /// <summary>
    ///     execute a process.
    /// </summary>
    /// <remarks>
    ///     Create a new job.  For more information, see [Section
    ///     7.11](https://docs.ogc.org/is/18-062/18-062.html#sc_create_job).
    /// </remarks>
    /// <param name="request">Mandatory execute request JSON</param>
    /// <param name="processId"></param>
    /// <response code="200">Result of synchronous execution</response>
    /// <response code="201">Started asynchronous execution. Created job.</response>
    /// <response code="404">The requested URI was not found.</response>
    /// <response code="500">A server error occurred.</response>
    [HttpPost]
    [Route("/processes/{processId}/execution")]
    [ValidateModelState]
    [SwaggerOperation("Execute")]
    [SwaggerResponse(200, type: typeof(IInlineResponse200), description: "Result of synchronous execution")]
    [SwaggerResponse(201, type: typeof(StatusInfo), description: "Started asynchronous execution. Created job.")]
    [SwaggerResponse(404, type: typeof(ExceptionResult), description: "The requested URI was not found.")]
    [SwaggerResponse(500, type: typeof(ExceptionResult), description: "A server error occurred.")]
    public async Task<IActionResult> Execute(
        [FromBody] Execute request, [FromRoute] [Required] string processId)
    {
        var create = new CreateSimulationJobRequest
        {
            SimulationId = processId, Execute = request
        };

        var response = await Mediator.Send(create);

        if (!string.IsNullOrEmpty(response.HangfireJobKey))
        {
            return StatusCode(201, response.Adapt<StatusInfo>());
        }

        ArgumentException.ThrowIfNullOrEmpty(response.ResultId);
        var result = await resultService.GetAsync(response.ResultId);
        return StatusCode(200, result.FeatureCollection);
    }
}