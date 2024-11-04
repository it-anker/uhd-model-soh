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
public class ConformanceDeclarationApiController : ControllerBase
{
    /// <summary>
    ///     information about standards that this API conforms to
    /// </summary>
    /// <remarks>
    ///     A list of all conformance classes, specified in a standard, that the server conforms to.  | Conformance class
    ///     | URI | |- -- -- -- -- --|- -- -- --| |Core|http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/core| |OGC
    ///     Process Description|http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/ogc-process-description|
    ///     |JSON|http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/json|
    ///     |HTML|http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/html| |OpenAPI Specification
    ///     3.0|http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/oas30| |Job
    ///     list|http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/job-list|
    ///     |Callback|http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/callback|
    ///     |Dismiss|http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/dismiss|  For more information, see [Section
    ///     7.4](https://docs.ogc.org/is/18-062/18-062.html#sc_conformance_classes).
    /// </remarks>
    /// <response code="200">
    ///     The URIs of all conformance classes supported by the server.  To support \&quot;generic\&quot;
    ///     clients that want to access multiple OGC API - Processes implementations - and not \&quot;just\&quot; a specific
    ///     API / server, the server declares the conformance classes it implements and conforms to.
    /// </response>
    /// <response code="500">A server error occurred.</response>
    [HttpGet]
    [Route("/ogcapi/conformance")]
    [ValidateModelState]
    [SwaggerOperation("GetConformanceClasses")]
    [SwaggerResponse(200, type: typeof(ConfClasses),
        description:
        "The URIs of all conformance classes supported by the server.  To support generic clients that want to access multiple OGC API - Processes implementations - and not just a specific API / server, the server declares the conformance classes it implements and conforms to.")]
    [SwaggerResponse(500, type: typeof(Exception), description: "A server error occurred.")]
    public virtual IActionResult GetConformanceClasses()
    {
        // TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(200, default(ConfClasses));

        // TODO: Uncomment the next line to return response 500 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(500, default(Exception));
        string? exampleJson = null;
        exampleJson =
            "{\n  \"conformsTo\" : [ \"http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/core\", \"http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/core\" ]\n}";

        ConfClasses? example = exampleJson != null
            ? JsonConvert.DeserializeObject<ConfClasses>(exampleJson)
            : default; // TODO: Change the data returned
        return new ObjectResult(example);
    }
}