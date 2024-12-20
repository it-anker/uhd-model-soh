using Mars.Common.Core.Collections;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SOH.Process.Server.Controllers.V1;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Simulations.Workflows;

public class ResultHttpFlow(
    IUrlHelper uriHelper,
    SimulationResultWorkflow resultWorkflow,
    JsonSerializerSettings serializerSettings,
    HttpContextAccessor contextAccessor) {

    public async Task<IActionResult> GetActionResult(string jobId, CancellationToken token = default)
    {
        var response = await resultWorkflow.RetrieveResultsAsync(
            new GetJobResultRequest { JobId = jobId }, token);

        if (response.RawSingleOutput != null)
        {
            return new OkObjectResult(response.RawSingleOutput);
        }

        if (response.DocumentOutput != null)
        {
            return new OkObjectResult(response.DocumentOutput);
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

            return new OkObjectResult(multipartResult);
        }

        var httpResponse = contextAccessor.HttpContext;
        if (httpResponse == null) return new OkResult();

        var links = (response.RawReferences ?? [])
            .Select(resultId => uriHelper.Action(action: nameof(ResultsController.GetResultAsync),
                controller: "Results", values: new { id = resultId }));

        foreach (string? uri in links)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                httpResponse.Response.Headers.Append("Link", uri);
            }
        }

        return new OkResult();
    }
}