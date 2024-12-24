using AutoMapper;
using Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesImagesController : ControllerBase
{
    private readonly DataContext dataContext;
    private readonly IMapper mapper;
    public GamesImagesController(DataContext dataContext, IMapper mapper)
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
    }

    [HttpGet("{name}")]
    public IActionResult Get(string name)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", name);

        if (System.IO.File.Exists(filePath))
        {
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "image/jpeg", name); // The third parameter is the file download name
        }
        else
        {
            return NotFound(); // Return 404 if file doesn't exist
        }
    }
}
