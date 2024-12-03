using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesDetailsPageController : ControllerBase
{
    [HttpGet("{title}")]
    public async Task<ActionResult<DetailsComponentItem>> GetDetailsComponentItemsAsync(string title)
    {
        return Ok();
    }
}
