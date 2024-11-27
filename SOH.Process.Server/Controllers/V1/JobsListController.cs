using Microsoft.AspNetCore.Mvc;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Common;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Simulations;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

[ApiController]
public class JobsListController(ISimulationService simulationService) : BaseApiController
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
    [Route("/jobs")]
    [ValidateModelState]
    [SwaggerOperation("GetJobs")]
    [SwaggerResponse(200, type: typeof(JobList), description: "A list of jobs for this process.")]
    [SwaggerResponse(404, type: typeof(ExceptionResult), description: "The requested URI was not found.")]
    public async Task<ActionResult<JobList>> GetJobs(CancellationToken token = default)
    {
        return Ok(await simulationService.ListJobsAsync(token));
    }
}