using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Common;
using SOH.Process.Server.Models.Processes;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

public class StatusController : BaseApiController
{
    /// <summary>
    ///     retrieve the status of a job.
    /// </summary>
    /// <remarks>
    ///     Shows the status of a job.   For more information, see [Section
    ///     7.12](https://docs.ogc.org/is/18-062/18-062.html#sc_retrieve_status_info).
    /// </remarks>
    /// <param name="jobId">local identifier of a job</param>
    /// <response code="200">The status of a job.</response>
    /// <response code="404">The requested URI was not found.</response>
    /// <response code="500">A server error occurred.</response>
    [HttpGet]
    [Route("/ogcapi/jobs/{jobId}")]
    [ValidateModelState]
    [SwaggerOperation("GetStatus")]
    [SwaggerResponse(200, type: typeof(StatusInfo), description: "The status of a job.")]
    [SwaggerResponse(404, type: typeof(ExceptionResult), description: "The requested URI was not found.")]
    [SwaggerResponse(500, type: typeof(ExceptionResult), description: "A server error occurred.")]
    public virtual IActionResult GetStatus([FromRoute] [Required] string jobId)
    {
        // TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(200, default(StatusInfo));

        // TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(404, default(Exception));

        // TODO: Uncomment the next line to return response 500 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(500, default(Exception));
        string? exampleJson = null;
        exampleJson =
            "{\n  \"jobID\" : \"jobID\",\n  \"processID\" : \"processID\",\n  \"created\" : \"2000-01-23T04:56:07.000+00:00\",\n  \"progress\" : 8,\n  \"started\" : \"2000-01-23T04:56:07.000+00:00\",\n  \"finished\" : \"2000-01-23T04:56:07.000+00:00\",\n  \"links\" : [ {\n    \"hreflang\" : \"en\",\n    \"rel\" : \"service\",\n    \"href\" : \"href\",\n    \"type\" : \"application/json\",\n    \"title\" : \"title\"\n  }, {\n    \"hreflang\" : \"en\",\n    \"rel\" : \"service\",\n    \"href\" : \"href\",\n    \"type\" : \"application/json\",\n    \"title\" : \"title\"\n  } ],\n  \"type\" : \"process\",\n  \"message\" : \"message\",\n  \"updated\" : \"2000-01-23T04:56:07.000+00:00\",\n  \"status\" : \"accepted\"\n}";

        StatusInfo? example = exampleJson != null
            ? JsonConvert.DeserializeObject<StatusInfo>(exampleJson)
            : default; // TODO: Change the data returned
        return new ObjectResult(example);
    }
}