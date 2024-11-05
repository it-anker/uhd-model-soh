using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

public class ProcessDescriptionApiApiController : BaseApiController
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
    /// <param name="processID"></param>
    /// <response code="200">A process description.</response>
    /// <response code="404">The requested URI was not found.</response>
    [HttpGet]
    [Route("/ogcapi/processes/{processID}")]
    [ValidateModelState]
    [SwaggerOperation("GetProcessDescription")]
    [SwaggerResponse(200, type: typeof(Models.Processes.Process), description: "A process description.")]
    [SwaggerResponse(404, type: typeof(ExceptionResult), description: "The requested URI was not found.")]
    public virtual IActionResult GetProcessDescription([FromRoute] [Required] string processID)
    {
        // TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(200, default(Process));

        // TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(404, default(Exception));
        string? exampleJson = null;
        exampleJson = "\"\"";

        Models.Processes.Process? example = exampleJson != null
            ? JsonConvert.DeserializeObject<Models.Processes.Process>(exampleJson)
            : default; // TODO: Change the data returned
        return new ObjectResult(example);
    }
}