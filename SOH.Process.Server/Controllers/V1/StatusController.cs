using System.ComponentModel.DataAnnotations;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Common;
using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Simulations;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

[ApiController]
public class StatusController(ISimulationService simulationService) : BaseApiController
{
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
    [HttpGet]
    [Route("/jobs/{jobId}")]
    [ValidateModelState]
    [SwaggerOperation("GetStatus")]
    [SwaggerResponse(200, type: typeof(StatusInfo), description: "The status of a job.")]
    [SwaggerResponse(404, type: typeof(ExceptionResult), description: "The requested URI was not found.")]
    [SwaggerResponse(500, type: typeof(ExceptionResult), description: "A server error occurred.")]
    public virtual async Task<ActionResult<StatusInfo>> GetStatus([FromRoute] [Required] string jobId)
    {
        var status = (await simulationService.GetSimulationJobAsync(jobId)).Adapt<StatusInfo>();
        return Ok(status);
    }
}