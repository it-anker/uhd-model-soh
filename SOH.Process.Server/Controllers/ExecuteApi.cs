using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Ogc;
using Swashbuckle.AspNetCore.Annotations;
using Exception = SOH.Process.Server.Models.Ogc.Exception;

namespace SOH.Process.Server.Controllers;

/// <summary>
/// </summary>
[ApiController]
public class ExecuteApiController : ControllerBase
{
    /// <summary>
    ///     execute a process.
    /// </summary>
    /// <remarks>
    ///     Create a new job.  For more information, see [Section
    ///     7.11](https://docs.ogc.org/is/18-062/18-062.html#sc_create_job).
    /// </remarks>
    /// <param name="body">Mandatory execute request JSON</param>
    /// <param name="processID"></param>
    /// <response code="200">Result of synchronous execution</response>
    /// <response code="201">Started asynchronous execution. Created job.</response>
    /// <response code="404">The requested URI was not found.</response>
    /// <response code="500">A server error occurred.</response>
    [HttpPost]
    [Route("/ogcapi/processes/{processID}/execution")]
    [ValidateModelState]
    [SwaggerOperation("Execute")]
    [SwaggerResponse(200, type: typeof(InlineResponse200), description: "Result of synchronous execution")]
    [SwaggerResponse(201, type: typeof(StatusInfo), description: "Started asynchronous execution. Created job.")]
    [SwaggerResponse(404, type: typeof(Exception), description: "The requested URI was not found.")]
    [SwaggerResponse(500, type: typeof(Exception), description: "A server error occurred.")]
    public virtual IActionResult Execute([FromBody] Execute body, [FromRoute] [Required] string processID)
    {
        // TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(200, default(InlineResponse200));

        // TODO: Uncomment the next line to return response 201 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(201, default(StatusInfo));

        // TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(404, default(Exception));

        // TODO: Uncomment the next line to return response 500 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(500, default(Exception));
        string? exampleJson = null;
        exampleJson = "\"\"";

        InlineResponse200? example = exampleJson != null
            ? JsonConvert.DeserializeObject<InlineResponse200>(exampleJson)
            : default; // TODO: Change the data returned
        return new ObjectResult(example);
    }
}