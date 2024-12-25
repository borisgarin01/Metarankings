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
    public async Task<IActionResult> Get(string name)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", name);

        if (System.IO.File.Exists(filePath))
        {
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "image/jpeg", name); // The third parameter is the file download name
        }
        else
        {
            return NotFound(); // Return 404 if file doesn't exist
        }
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        // Define the path where the file will be saved
        var uploadsFolder = "Images";
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder); // Create the directory if it doesn't exist
        }

        var filePath = Path.Combine(uploadsFolder, file.FileName);

        // Save the file to the server
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        return Created($"api/GamesImages/{file.FileName}", file);
    }
}
