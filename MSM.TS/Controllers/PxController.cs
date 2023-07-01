using Microsoft.AspNetCore.Mvc;
using MSM.Common.Computer;
using MSM.Common.Controllers;
using MSM.Common.Enums;
using MSM.Common.Utils;
using MSM.TS.Payloads;

namespace MSM.TS.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PxController : ControllerBase {
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
    public async Task<FileResult> GetCalculatedBars(
        [FromQuery] string item,
        [FromQuery] DateTime start,
        [FromQuery] DateTime? end,
        [FromQuery] int intervalMin
    ) {
        using var stream = new MemoryStream();
        await using TextWriter writer = new StreamWriter(stream);

        await writer.WriteLineAsync("Time,Open,High,Low,Close,UpTick,DownTick");
        var bars = PxDataAggregator.GetBarsAsync(item, start, end ?? DateTime.UtcNow, intervalMin);

        foreach (var row in await bars) {
            await writer.WriteLineAsync(
                $"{row.Timestamp},{row.Open},{row.High},{row.Low},{row.Close},{row.UpTick},{row.DownTick}"
            );
        }

        await writer.FlushAsync();
        stream.Position = 0;

        return File(
            stream.ToArray(),
            "text/csv",
            $"{item}-{start:yyyyMMdd}-{end:yyyyMMdd}@{intervalMin}.csv"
        );
    }
}