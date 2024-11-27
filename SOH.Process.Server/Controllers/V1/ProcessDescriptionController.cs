using System.ComponentModel.DataAnnotations;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Common;
using SOH.Process.Server.Simulations;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

[ApiController]
public class ProcessDescriptionController(ISimulationService simulationService) : BaseApiController
{
    /// <summary>
    ///     retrieve a process description
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
    [HttpGet]
    [Route("/processes/{processID}")]
    [ValidateModelState]
    [SwaggerOperation("GetProcessDescription")]
    [SwaggerResponse(200, type: typeof(Models.Processes.Process), description: "A process description.")]
    [SwaggerResponse(404, type: typeof(ExceptionResult), description: "The requested URI was not found.")]
    public virtual async Task<ActionResult<Models.Processes.Process>> GetProcessDescription(
        [FromRoute] [Required] string processID, CancellationToken token)
    {
        var process = await simulationService.GetSimulationAsync(processID, token);
        return Ok(process.Adapt<Models.Processes.Process>());
    }
}