namespace API.Controllers.Movies;

[Route("api/movies/[controller]")]
public sealed class ImagesController : ImagesControllerBase
{
    public ImagesController(IWebHostEnvironment webHostEnvironment)
        : base(webHostEnvironment, "movies", "/api/movies/Images")
    {
    }
}
