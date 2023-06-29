using Microsoft.AspNetCore.Mvc;
using MSM.Common.Controllers;
using MSM.Common.Utils;
using MSM.TS.Payloads;

namespace MSM.TS.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PxController : ControllerBase {
    private readonly ILogger<PxController> _logger;

    public PxController(ILogger<PxController> logger) {
        _logger = logger;
    }

    [HttpPost]
    public async Task<StatusCodeResult> RecordPx([FromForm] PxRecordPayload payload) {
        if (payload.Token != ConfigHelper.GetApiToken()) {
            return Unauthorized();
        }

        _logger.LogInformation("Received Px record of {Item} at {Px}", payload.Item, payload.Px);

        await PxTickController.RecordPx(payload.Item, payload.Px);
        return Ok();
    }
}