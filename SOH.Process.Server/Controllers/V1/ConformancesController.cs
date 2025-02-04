using Microsoft.AspNetCore.Mvc;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Ogc;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

public class ConformanceController : BaseApiRouteController
{
    /// <summary>
    ///     Retrieve the set of OGC API conformance classes that are supported by this service.
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
    [ValidateModelState]
    [SwaggerOperation("GetConformanceClasses")]
    [SwaggerResponse(200, type: typeof(ConfClasses),
        description:
        "The URIs of all conformance classes supported by the server.  To support generic clients that want to access multiple OGC API - Processes implementations - and not just a specific API / server, the server declares the conformance classes it implements and conforms to.")]
    [SwaggerResponse(500, type: typeof(ProblemDetails), description: "A server error occurred.")]
    public virtual IActionResult GetConformanceClasses()
    {
        var classes = new ConfClasses
        {
            ConformsTo =
            [
                "http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/core", // core feature
                "http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/ogc-process-description", // process desc feature
                "http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/json", // json feature
                "http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/oas30", // openapi feature
                "http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/job-list", // job list feature

                //"http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/callback", // callback feature
                "http://www.opengis.net/spec/ogcapi-processes-1/1.0/conf/dismiss]", // dismiss feature
            ]
        };

        return Ok(classes);
    }
}