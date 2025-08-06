using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.Developers;
using ExcelProcessors;
using IdentityLibrary.Telegram;
using Telegram.Bot.Types;

namespace API.Controllers.Games;

[ApiController]
[Route("api/[controller]")]
public sealed class DevelopersController : ControllerBase
{
    private readonly IMapper _mapper;

    private readonly IRepository<Developer> _developersRepository;
    private readonly IExcelDataReader<Developer> _developersExcelDataReader;

    private readonly IWebHostEnvironment _webHostEnvironment;

    private readonly ILogger<DevelopersController> _logger;

    private readonly TelegramAuthenticator _telegramAuthenticator;

    public DevelopersController(IMapper mapper, IRepository<Developer> developersRepository, ILogger<DevelopersController> logger, IExcelDataReader<Developer> developersExcelDataReader, TelegramAuthenticator telegramAuthenticator)
    {
        _mapper = mapper;

        _developersRepository = developersRepository;
        _developersExcelDataReader = developersExcelDataReader;

        _logger = logger;
        _telegramAuthenticator = telegramAuthenticator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Developer>>> GetAllAsync()
    {
        var developers = await _developersRepository.GetAllAsync();

        return Ok(developers);
    }

    [HttpGet("{offset:long}/{limit:long}")]
    public async Task<ActionResult<IEnumerable<Developer>>> GetAsync(long offset, long limit)
    {
        var developers = await _developersRepository.GetAsync(offset, limit);

        return Ok(developers);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<Developer>> AddAsync(AddDeveloperModel addDeveloperModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var developer = _mapper.Map<Developer>(addDeveloperModel);

        var insertedDeveloperId = await _developersRepository.AddAsync(developer);

        developer = developer with { Id = insertedDeveloperId };

        await _telegramAuthenticator.SendMessageAsync($"New developer {developer.Name} at {this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/api/developers/{developer.Id}");

        return Created($"api/developers/{developer.Id}", developer);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Developer>> GetAsync(long id)
    {
        var developer = await _developersRepository.GetAsync(id);
        if (developer is null)
            return NotFound();
        else
            return Ok(developer);
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        var developer = await _developersRepository.GetAsync(id);
        if (developer is null)
            return NotFound();
        else
        {
            try
            {
                await _developersRepository.RemoveAsync(developer.Id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<Developer>> UpdateAsync(long id, UpdateDeveloperModel updateDeveloperModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var developerToUpdate = await _developersRepository.GetAsync(id);
        if (developerToUpdate is null)
            return NotFound();

        // Map the update model to the existing entity
        var developerToGetAfterUpdate = _mapper.Map<Developer>(updateDeveloperModel);

        // Update and return the updated entity
        var updatedDeveloper = await _developersRepository.UpdateAsync(developerToGetAfterUpdate, id);

        return Ok(updatedDeveloper);
    }

    [HttpPost("upload-developers-from-json")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult> AddFromJsonAsync(IEnumerable<Developer> developers)
    {
        if (developers is null)
            return Problem("Developers don't set", null, 400);

        if (!developers.Any())
            return Problem("Developers array is empty", null, 400);

        try
        {
            await _developersRepository.AddRangeAsync(developers);

            return Ok();
        }
        catch (Exception ex)
        {
            if (_webHostEnvironment.IsDevelopment())
            {
                return StatusCode(500, ex);
            }
            else
            {
                _logger.LogError(ex.Message, ex.StackTrace);
                return StatusCode(500, new { Message = "Something goes wrong" });
            }
        }
    }

    [HttpPost("developers-excel-upload")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<IEnumerable<Developer>>> AddFromExcelAsync(IFormFile excelFileWithPublishers)
    {
        if (excelFileWithPublishers is null)
            return Problem("File hasn't set", null, 400);

        if (!excelFileWithPublishers.FileName.EndsWith(".xlsx") && !excelFileWithPublishers.FileName.EndsWith(".xlsx"))
            return Problem("This is not an Excel file", null, 400);

        try
        {
            string uploadsFolderPath = $"{Directory.GetCurrentDirectory()}\\Uploads";

            if (!Directory.Exists(uploadsFolderPath))
                Directory.CreateDirectory(uploadsFolderPath);

            string filePath = Path.Combine(uploadsFolderPath, excelFileWithPublishers.FileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await excelFileWithPublishers.CopyToAsync(fileStream);
            }

            IEnumerable<Developer> developersToUpload = _developersExcelDataReader.GetFromExcel(filePath);

            try
            {
                await _developersRepository.AddRangeAsync(developersToUpload);

                System.IO.File.Delete(filePath);

                return Ok();
            }
            catch (Exception ex)
            {
                if (_webHostEnvironment.IsDevelopment())
                {
                    return StatusCode(500, ex);
                }
                else
                {
                    _logger.LogError(ex.Message, ex.StackTrace);
                    return StatusCode(500, new { Message = "Something goes wrong" });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.StackTrace);
            return StatusCode(500, new { Message = "Something goes wrong" });
        }
    }
}
