using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models.Common;
using SOH.Process.Server.Models.Ogc;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

public class CapabilitiesApiApiController : BaseApiController
{
    /// <summary>
    ///     landing page of this API.
    /// </summary>
    /// <remarks>
    ///     The landing page provides links to the:   * The APIDefinition (no fixed path),   * The Conformance statements
    ///     (path /conformance),   * The processes metadata (path /processes),   * The endpoint for job monitoring (path
    ///     /jobs).  For more information, see [Section 7.2](https://docs.ogc.org/is/18-062/18-062.html#sc_landing_page).
    /// </remarks>
    /// <response code="200">
    ///     The landing page provides links to the API definition (link relations &#x60;service-desc&#x60; and
    ///     &#x60;service-doc&#x60;), the Conformance declaration (path &#x60;/conformance&#x60;, link relation &#x60;
    ///     http://www.opengis.net/def/rel/ogc/1.0/conformance&#x60;), and to other resources.
    /// </response>
    /// <response code="500">A server error occurred.</response>
    [HttpGet]
    [Route("/ogcapi/")]
    [ValidateModelState]
    [SwaggerOperation("GetLandingPage")]
    [SwaggerResponse(200, type: typeof(LandingPage),
        description:
        "The landing page provides links to the API definition (link relations &#x60;service-desc&#x60; and &#x60;service-doc&#x60;), the Conformance declaration (path &#x60;/conformance&#x60;, link relation &#x60;http://www.opengis.net/def/rel/ogc/1.0/conformance&#x60;), and to other resources.")]
    [SwaggerResponse(500, type: typeof(ExceptionResult), description: "A server error occurred.")]
    public IActionResult GetLandingPage()
    {
        // TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(200, default(LandingPage));

        // TODO: Uncomment the next line to return response 500 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
        // return StatusCode(500, default(Exception));
        string? exampleJson =
            "{\n  \"description\" : \"Example server implementing the OGC API - Processes 1.0 Standard\",\n  \"links\" : [ {\n    \"hreflang\" : \"en\",\n    \"rel\" : \"service\",\n    \"href\" : \"href\",\n    \"type\" : \"application/json\",\n    \"title\" : \"title\"\n  }, {\n    \"hreflang\" : \"en\",\n    \"rel\" : \"service\",\n    \"href\" : \"href\",\n    \"type\" : \"application/json\",\n    \"title\" : \"title\"\n  } ],\n  \"title\" : \"Example processing server\"\n}";

        LandingPage? example = exampleJson != null
            ? JsonConvert.DeserializeObject<LandingPage>(exampleJson)
            : default; // TODO: Change the data returned
        return new ObjectResult(example);
    }
}