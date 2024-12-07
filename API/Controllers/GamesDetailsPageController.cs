using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesDetailsPageController : ControllerBase
{
    private readonly DataContext dataContext;

    public GamesDetailsPageController(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    [HttpGet("{title}")]
    public async Task<ActionResult<DetailsComponentItem>> GetDetailsComponentItemsAsync(string title)
    {
        var a = dataContext.DetailsComponentsItems.ToArray();
        var b = title;
        var detailsComponentItem = await dataContext.DetailsComponentsItems.FirstOrDefaultAsync(x => x.Name.Contains(title));
        if (detailsComponentItem is null)
            return NotFound();
        return Ok(detailsComponentItem);
    }
}
