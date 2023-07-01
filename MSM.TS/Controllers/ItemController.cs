using Microsoft.AspNetCore.Mvc;
using MSM.Common.Controllers;
using MSM.TS.Responses;

namespace MSM.TS.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ItemController : ControllerBase {
    [HttpGet]
    public async Task<JsonResult> GetAvailableItems() {
        return new JsonResult(new AvailableItemsResponse {
            Items = (await PxTickController.GetAvailableItemsAsync()).Order()
        });
    }
}