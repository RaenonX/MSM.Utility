using Microsoft.AspNetCore.Mvc;
using MSM.Common.Controllers;
using MSM.TS.Responses;

namespace MSM.TS.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ItemController : ControllerBase {
    private readonly ILogger<ItemController> _logger;

    public ItemController(ILogger<ItemController> logger) {
        _logger = logger;
    }

    [HttpGet]
    [Route("available")]
    public async Task<JsonResult> GetAvailableItems() {
        _logger.LogInformation("Getting available items");

        return new JsonResult(new AvailableItemsResponse {
            Items = (await PxTickController.GetAvailableItemsAsync())
        });
    }

    [HttpGet]
    [Route("tracking")]
    public async Task<ActionResult> GetTrackingItems() {
        _logger.LogInformation("Getting tracking items");

        return Content(string.Join(',', (await PxTrackingItemController.GetTrackingItemsAsync()).Select(x => x.Item)));
    }

    [HttpGet]
    [Route("sniping")]
    public async Task<ActionResult> GetSnipingItem() {
        _logger.LogInformation("Getting sniping items");
        var snipingItem = await PxSnipingItemController.GetSnipingItemAsync();

        return Content(snipingItem?.Item ?? "");
    }
}