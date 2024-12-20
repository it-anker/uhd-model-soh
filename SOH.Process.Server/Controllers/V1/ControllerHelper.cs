using Mars.Common.Core.Collections;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Controllers.V1;

public static class ControllerHelper
{
    public static IActionResult GetActionResult(
        this BaseApiController controller, LinkGenerator linkGenerator,
        HttpContext context, JobResultResponse response,
        JsonSerializerSettings serializerSettings)
    {
        if (response.RawSingleOutput != null)
        {
            return controller.Ok(response.RawSingleOutput);
        }

        if (response.DocumentOutput != null)
        {
            return controller.Ok(response.DocumentOutput);
        }

        if (response.RawMultiOutput != null)
        {
            var multipartResult = new MultipartContent("related");

            foreach (object result in response.RawMultiOutput.WhereNotNull())
            {
                string json = JsonConvert.SerializeObject(result, serializerSettings);
                var content = new StringContent(json);
                multipartResult.Add(content);
            }

            return controller.Ok(multipartResult);
        }

        var links = (response.RawReferences ?? [])
            .Select(resultId => linkGenerator.GetUriByAction(context,
                action: nameof(ResultsController.GetResultAsync),
                controller: "Results", values: new { id = resultId }));

        foreach (string? uri in links)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                context.Response.Headers.Append("Link", uri);
            }
        }

        return controller.Ok();
    }
}