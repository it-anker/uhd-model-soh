using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SOH.Process.Server.Attributes;
using SOH.Process.Server.Models;
using SOH.Process.Server.Models.Common;
using SOH.Process.Server.Models.Ogc;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Controllers.V1;

[ApiController]
public class CapabilitiesController(IOptions<OgcSettings> configuration) : BaseApiController
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
    [Route("/")]
    [ValidateModelState]
    [SwaggerOperation("GetLandingPage")]
    [SwaggerResponse(200, type: typeof(LandingPage),
        description:
        "The landing page provides links to the API definition (link relations &#x60;service-desc&#x60; and &#x60;service-doc&#x60;), the Conformance declaration (path &#x60;/conformance&#x60;, link relation &#x60;http://www.opengis.net/def/rel/ogc/1.0/conformance&#x60;), and to other resources.")]
    [SwaggerResponse(500, type: typeof(ExceptionResult), description: "A server error occurred.")]
    public ActionResult<LandingPage> GetLandingPage()
    {
        return Ok(configuration.Value.Capabilities);
    }
}