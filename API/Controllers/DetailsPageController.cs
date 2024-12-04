using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
