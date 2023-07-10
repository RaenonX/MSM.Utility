using Microsoft.AspNetCore.Mvc;
using MSM.Common.Controllers;
using MSM.Common.Utils;
using MSM.TS.Payloads;
using MSM.TS.Responses;

namespace MSM.TS.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ScriptController : ControllerBase {
    private readonly ILogger<ScriptController> _logger;

    public ScriptController(ILogger<ScriptController> logger) {
        _logger = logger;
    }

    [HttpPost]
    [Route("loop")]
    public async Task<ActionResult> RecordLoopStats([FromForm] ScriptLoopRecordPayload payload) {
        if (payload.Token != ConfigHelper.GetApiToken()) {
            return Unauthorized();
        }

        _logger.LogInformation(
            "Recorded script looped once in {Elapsed:0.00} secs for {ItemCount} items",
            payload.Elapsed,
            payload.Count
        );
        await ScriptLoopTimeController.RecordLoopTime(payload.Count, payload.Elapsed);

        return Ok();
    }

    [HttpGet]
    [Route("loop")]
    public async Task<JsonResult> GetScriptLoopStats([FromQuery] int loops) {
        var avgPerItem = await ScriptLoopTimeController.GetAvgPerItem(loops);

        _logger.LogInformation(
            "Returning script loop stats in {LoopCount} loops ({AvgPerItem:0.000} secs / item)",
            loops,
            avgPerItem
        );

        return new JsonResult(new ScriptLoopStatsResponse {
            AvgItemSec = avgPerItem
        });
    }
}