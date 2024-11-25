using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Common;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

public class ExecuteController : BaseApiController
{
    /// <summary>
    ///     execute a process.
    /// </summary>
    /// <remarks>
    ///     Create a new job.  For more information, see [Section
    ///     7.11](https://docs.ogc.org/is/18-062/18-062.html#sc_create_job).
    /// </remarks>
    /// <param name="body">Mandatory execute request JSON</param>
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
    public async Task<ActionResult<StatusCode>> Execute([FromBody] Execute body, [FromRoute] [Required] string processId)
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

        var example = exampleJson != null
            ? JsonConvert.DeserializeObject<IInlineResponse200>(exampleJson)
            : default; // TODO: Change the data returned
        return Ok(example);
    }
}