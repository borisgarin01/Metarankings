using Data;
using Microsoft.AspNetCore.Mvc;

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


}
