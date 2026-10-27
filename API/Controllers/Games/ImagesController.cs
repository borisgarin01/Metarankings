namespace API.Controllers.Games;

[Route("api/games/[controller]")]
public sealed class ImagesController : ImagesControllerBase
{
    public ImagesController(IWebHostEnvironment webHostEnvironment)
        : base(webHostEnvironment, "games", "/api/games/Images")
    {
    }
}
