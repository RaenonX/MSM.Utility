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
    public async Task<JsonResult> GetAvailableItems() {
        _logger.LogInformation("Getting available items");

        return new JsonResult(new AvailableItemsResponse {
            Items = (await PxTickController.GetAvailableItemsAsync()).Order()
        });
    }
}