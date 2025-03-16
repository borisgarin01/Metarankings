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

    [HttpGet("{pageNumber}/{pageSize}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetAsync(ushort pageNumber = 1, byte pageSize = 5)
    {
        using (var fileStream = new FileStream("Games.json", FileMode.Open, FileAccess.Read))
        {

            var games = await JsonSerializer.DeserializeAsync<IEnumerable<Game>>(fileStream, jsonSerializerOptions);
            games = games!.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return Ok(games);
        }
    }
}
