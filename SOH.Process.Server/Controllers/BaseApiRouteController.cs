using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SOH.Process.Server.Controllers;

[ApiController]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "ogc-processes")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}

[Route("[controller]")]
public abstract class BaseApiRouteController : BaseApiController;