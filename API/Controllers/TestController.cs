namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Get");
    }

    [HttpPost]
    public IActionResult Post()
    {
        return Ok("Post");
    }
}
