using API.Models.RequestsModels.Games.Publishers;
using Data.Repositories.Interfaces;
using Domain.Games;
using ExcelProcessors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace API.Controllers.Games;

[ApiController]
[Route("api/[controller]")]
public sealed class PublishersController : ControllerBase
{
    private readonly IMapper _mapper;

    private readonly IRepository<Publisher> _publishersRepository;

    private readonly IValidator<AddPublisherModel> _addPublisherValidator;
    private readonly IValidator<UpdatePublisherModel> _updatePublisherValidator;

    private readonly IExcelDataReader<Publisher> _publishersExcelDataReader;

    private readonly IWebHostEnvironment _webHostEnvironment;

    private readonly ILogger<PublishersController> _logger;

    public PublishersController(IMapper mapper, IRepository<Publisher> publishersRepository, IValidator<AddPublisherModel> addPublisherValidator, IValidator<UpdatePublisherModel> pdatePublisherValidator, IExcelDataReader<Publisher> publishersExcelDataReader, IWebHostEnvironment webHostEnvironment, ILogger<PublishersController> logger)
    {
        _mapper = mapper;
        _publishersRepository = publishersRepository;
        _addPublisherValidator = addPublisherValidator;
        _updatePublisherValidator = pdatePublisherValidator;
        _publishersExcelDataReader = publishersExcelDataReader;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;

    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Publisher>>> GetAllAsync()
    {
        var publishers = await _publishersRepository.GetAllAsync();

        return Ok(publishers);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<Publisher>> AddAsync(AddPublisherModel addPublisherModel)
    {
        var validationResult = _addPublisherValidator.Validate(addPublisherModel);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult);
        }

        var publisher = _mapper.Map<Publisher>(addPublisherModel);

        var insertedPublisherId = await _publishersRepository.AddAsync(publisher);

        publisher.Id = insertedPublisherId;

        return Created($"api/publishers/{publisher.Id}", publisher);
    }

    [HttpPost("publishers-excel-upload")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<IEnumerable<Publisher>>> AddFromExcelAsync(IFormFile excelFileWithPublishers)
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

            IEnumerable<Publisher> publishersToUpload = _publishersExcelDataReader.GetFromExcel(filePath);

            try
            {
                await _publishersRepository.AddRangeAsync(publishersToUpload);

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

    [HttpPost("upload-publishers-from-json")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult> AddFromJsonAsync(IEnumerable<Publisher> publishers)
    {
        if (publishers is null)
            return Problem("Publishers don't set", null, 400);

        if (!publishers.Any())
            return Problem("Publishers array is empty", null, 400);

        try
        {
            await _publishersRepository.AddRangeAsync(publishers);

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


    [HttpGet("{id:long}")]
    public async Task<ActionResult<Publisher>> GetAsync(long id)
    {
        var publisher = await _publishersRepository.GetAsync(id);
        if (publisher is null)
            return NotFound();
        else
            return Ok(publisher);
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        var publisher = await _publishersRepository.GetAsync(id);
        if (publisher is null)
            return NotFound();
        else
        {
            try
            {
                await _publishersRepository.RemoveAsync(publisher.Id);
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
    public async Task<ActionResult<Publisher>> UpdateAsync(long id, UpdatePublisherModel updatePublisherModel)
    {
        var validationResult = _updatePublisherValidator.Validate(updatePublisherModel);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult);
        }

        var publisherToUpdate = await _publishersRepository.GetAsync(id);
        if (publisherToUpdate is null)
            return NotFound();

        var publisherToGetAfterUpdate = _mapper.Map<Publisher>(updatePublisherModel);

        var updatePublisher = await _publishersRepository.UpdateAsync(publisherToGetAfterUpdate, id);

        return Ok(updatePublisher);
    }
}
