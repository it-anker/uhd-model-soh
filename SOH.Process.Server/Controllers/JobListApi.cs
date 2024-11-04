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
public class JobListApiController : ControllerBase
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
    [Route("/ogcapi/jobs")]
    [ValidateModelState]
    [SwaggerOperation("GetJobs")]
    [SwaggerResponse(200, type: typeof(JobList), description: "A list of jobs for this process.")]
    [SwaggerResponse(404, type: typeof(Exception), description: "The requested URI was not found.")]
    public virtual IActionResult GetJobs()
    {
        // TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(200, default(JobList));

        // TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(404, default(Exception));
        string? exampleJson = null;
        exampleJson =
            "{\n  \"jobs\" : [ {\n    \"jobID\" : \"jobID\",\n    \"processID\" : \"processID\",\n    \"created\" : \"2000-01-23T04:56:07.000+00:00\",\n    \"progress\" : 8,\n    \"started\" : \"2000-01-23T04:56:07.000+00:00\",\n    \"finished\" : \"2000-01-23T04:56:07.000+00:00\",\n    \"links\" : [ {\n      \"hreflang\" : \"en\",\n      \"rel\" : \"service\",\n      \"href\" : \"href\",\n      \"type\" : \"application/json\",\n      \"title\" : \"title\"\n    }, {\n      \"hreflang\" : \"en\",\n      \"rel\" : \"service\",\n      \"href\" : \"href\",\n      \"type\" : \"application/json\",\n      \"title\" : \"title\"\n    } ],\n    \"type\" : \"process\",\n    \"message\" : \"message\",\n    \"updated\" : \"2000-01-23T04:56:07.000+00:00\",\n    \"status\" : \"accepted\"\n  }, {\n    \"jobID\" : \"jobID\",\n    \"processID\" : \"processID\",\n    \"created\" : \"2000-01-23T04:56:07.000+00:00\",\n    \"progress\" : 8,\n    \"started\" : \"2000-01-23T04:56:07.000+00:00\",\n    \"finished\" : \"2000-01-23T04:56:07.000+00:00\",\n    \"links\" : [ {\n      \"hreflang\" : \"en\",\n      \"rel\" : \"service\",\n      \"href\" : \"href\",\n      \"type\" : \"application/json\",\n      \"title\" : \"title\"\n    }, {\n      \"hreflang\" : \"en\",\n      \"rel\" : \"service\",\n      \"href\" : \"href\",\n      \"type\" : \"application/json\",\n      \"title\" : \"title\"\n    } ],\n    \"type\" : \"process\",\n    \"message\" : \"message\",\n    \"updated\" : \"2000-01-23T04:56:07.000+00:00\",\n    \"status\" : \"accepted\"\n  } ],\n  \"links\" : [ null, null ]\n}";

        JobList? example = exampleJson != null
            ? JsonConvert.DeserializeObject<JobList>(exampleJson)
            : default; // TODO: Change the data returned
        return new ObjectResult(example);
    }
}