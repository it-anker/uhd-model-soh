using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

public class ResultController : BaseApiController
{
    /// <summary>
    ///     retrieve the result(s) of a job
    /// </summary>
    /// <remarks>
    ///     Lists available results of a job. In case of a failure, lists exceptions instead.  For more information, see
    ///     [Section 7.13](https://docs.ogc.org/is/18-062/18-062.html#sc_retrieve_job_results).
    /// </remarks>
    /// <param name="jobId">local identifier of a job</param>
    /// <response code="200">The results of a job.</response>
    /// <response code="404">The requested URI was not found.</response>
    /// <response code="500">A server error occurred.</response>
    [HttpGet]
    [Route("/ogcapi/jobs/{jobId}/results")]
    [ValidateModelState]
    [SwaggerOperation("GetResult")]
    [SwaggerResponse(200, type: typeof(Results), description: "The results of a job.")]
    [SwaggerResponse(404, type: typeof(ExceptionResult), description: "The requested URI was not found.")]
    [SwaggerResponse(500, type: typeof(ExceptionResult), description: "A server error occurred.")]
    public virtual IActionResult GetResult([FromRoute] [Required] string jobId)
    {
        // TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(200, default(Results));

        // TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(404, default(Exception));

        // TODO: Uncomment the next line to return response 500 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(500, default(Exception));
        string? exampleJson = null;
        exampleJson = "{\n  \"key\" : \"\"\n}";

        Models.Ogc.Results? example = exampleJson != null
            ? JsonConvert.DeserializeObject<Models.Ogc.Results>(exampleJson)
            : default; // TODO: Change the data returned
        return new ObjectResult(example);
    }
}