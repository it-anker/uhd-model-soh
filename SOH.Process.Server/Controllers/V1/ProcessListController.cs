using Microsoft.AspNetCore.Mvc;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Simulations;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

[ApiController]
public class ProcessListController(ISimulationService simulationService) : BaseApiController
{
    /// <summary>
    ///     retrieve the list of available processes.
    /// </summary>
    /// <remarks>
    ///     The list of processes contains a summary of each process the OGC API - Processes offers, including the link to
    ///     a more detailed description of the process.  For more information, see [Section
    ///     7.9](https://docs.ogc.org/is/18-062/18-062.html#sc_process_list).
    /// </remarks>
    /// <response code="200">Information about the available processes.</response>
    [HttpGet]
    [Route("/processes")]
    [ValidateModelState]
    [SwaggerOperation("GetProcesses")]
    [SwaggerResponse(200, type: typeof(ProcessList), description: "Information about the available processes")]
    public async Task<ActionResult<ProcessList>> GetProcesses(
        [FromQuery(Name = "limit")] int? limit,
        CancellationToken token = default)
    {
        return Ok(await simulationService.ListProcessesAsync(
            new ParameterLimit { PageSize = limit.GetValueOrDefault() }, token));
    }
}