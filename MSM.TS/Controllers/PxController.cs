using Microsoft.AspNetCore.Mvc;
using MSM.Common.Computer;
using MSM.Common.Controllers;
using MSM.Common.Enums;
using MSM.Common.Utils;
using MSM.TS.Payloads;
using MSM.TS.Responses;

namespace MSM.TS.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PxController : ControllerBase {
    private readonly ILogger<PxController> _logger;

    public PxController(ILogger<PxController> logger) {
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> RecordPx([FromForm] PxRecordPayload payload) {
        if (payload.Token != ConfigHelper.GetApiToken()) {
            return Unauthorized();
        }

        var result = await PxTickController.RecordPx(payload.Item, payload.Px);

        return result switch {
            PxRecordResult.Recorded => Ok(),
            PxRecordResult.RecordedWithQueueAborted => Ok(),
            PxRecordResult.VerificationPending => Ok(payload.Item),
            _ => BadRequest($"Unhandled record result: {result}")
        };
    }

    [HttpGet]
    public async Task<JsonResult> GetCalculatedBars(
        [FromQuery] string item,
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end,
        [FromQuery] int intervalMin
    ) {
        _logger.LogInformation(
            "Getting calculated bars of {Item} in {IntervalMin} mins interval ({Start} ~ {End})",
            item,
            intervalMin,
            start,
            end
        );

        return new JsonResult(new PxBarResponse {
            Item = item,
            Bars = await PxDataAggregator.GetBarsAsync(item, start, end, intervalMin)
        });
    }
}