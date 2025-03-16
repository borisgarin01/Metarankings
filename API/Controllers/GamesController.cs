using API.Json;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesController : ControllerBase
{
    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

    public GamesController()
    {
        jsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter("yyyy-MM-dd"));
    }

    [HttpGet("{pageNumber:int}/{pageSize:int}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetAsync(int pageNumber = 1, int pageSize = 5)
    {
        using (var fileStream = new FileStream("Games.json", FileMode.Open, FileAccess.Read))
        {
            var games = await JsonSerializer.DeserializeAsync<IEnumerable<Game>>(fileStream, jsonSerializerOptions);
            games = games!.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return Ok(games);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> GetAsync()
    {
        using (var fileStream = new FileStream("Games.json", FileMode.Open, FileAccess.Read))
        {
            var games = await JsonSerializer.DeserializeAsync<IEnumerable<Game>>(fileStream, jsonSerializerOptions);
            return Ok(games);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Game>> GetAsync(int id)
    {
        using (var fileStream = new FileStream("Games.json", FileMode.Open, FileAccess.Read))
        {
            var games = await JsonSerializer.DeserializeAsync<IEnumerable<Game>>(fileStream, jsonSerializerOptions);
            var game = games.FirstOrDefault(a => a.Id == id);
            if (game is null)
                return NotFound();
            return Ok(game);
        }
    }
}
