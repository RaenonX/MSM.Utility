using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MSM.Common.Controllers;
using MSM.Common.Utils;
using MSM.TS.Payloads;

namespace MSM.TS.Controllers;

[ApiController]
[Route("/api/px")]
public class ApiPxController : ControllerBase {
    private readonly ILogger<ApiPxController> _logger;

    public ApiPxController(ILogger<ApiPxController> logger) {
        _logger = logger;
    }

    [HttpPost]
    public async Task<StatusCodeResult> RecordPx(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] PxRecordPayload payload
    ) {
        if (payload.Token != ConfigHelper.GetApiToken()) {
            return Unauthorized();
        }

        _logger.LogInformation("Received price record of {Item} at {Px}", payload.Item, payload.Px);

        await PxController.RecordPx(payload.Item, payload.Px);
        return Ok();
    }
}