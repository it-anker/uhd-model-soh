using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Processes;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

public class ProcessListApiApiController : BaseApiController
{
    /// <summary>
    ///     retrieve the list of available processes
    /// </summary>
    /// <remarks>
    ///     The list of processes contains a summary of each process the OGC API - Processes offers, including the link to
    ///     a more detailed description of the process.  For more information, see [Section
    ///     7.9](https://docs.ogc.org/is/18-062/18-062.html#sc_process_list).
    /// </remarks>
    /// <response code="200">Information about the available processes</response>
    [HttpGet]
    [Route("/ogcapi/processes")]
    [ValidateModelState]
    [SwaggerOperation("GetProcesses")]
    [SwaggerResponse(200, type: typeof(ProcessList), description: "Information about the available processes")]
    public virtual IActionResult GetProcesses()
    {
        // TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(200, default(ProcessList));
        string? exampleJson = null;
        exampleJson =
            "{\n  \"processes\" : [ \"\", \"\" ],\n  \"links\" : [ {\n    \"hreflang\" : \"en\",\n    \"rel\" : \"service\",\n    \"href\" : \"href\",\n    \"type\" : \"application/json\",\n    \"title\" : \"title\"\n  }, {\n    \"hreflang\" : \"en\",\n    \"rel\" : \"service\",\n    \"href\" : \"href\",\n    \"type\" : \"application/json\",\n    \"title\" : \"title\"\n  } ]\n}";

        ProcessList? example = exampleJson != null
            ? JsonConvert.DeserializeObject<ProcessList>(exampleJson)
            : default; // TODO: Change the data returned
        return new ObjectResult(example);
    }
}