namespace API.Controllers;

/// <summary>
/// Загрузка и раздача изображений раздела, хранящихся в {ContentRoot}/{section}/{year}/{month}.
/// </summary>
[ApiController]
public abstract class ImagesControllerBase : ControllerBase
{
    private static readonly Dictionary<string, string> ContentTypesByExtension = new()
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".svg"] = "image/svg+xml",
        [".gif"] = "image/gif",
        [".tiff"] = "image/tiff",
        [".webp"] = "image/webp",
        [".bmp"] = "image/bmp",
        [".heif"] = "image/heif",
    };

    private readonly string _baseImagesPath;
    private readonly string _baseUrl;

    /// <summary>
    /// Создаёт контроллер для раздела <paramref name="section"/> с публичным адресом <paramref name="baseUrl"/>.
    /// </summary>
    protected ImagesControllerBase(IWebHostEnvironment webHostEnvironment, string section, string baseUrl)
    {
        _baseImagesPath = Path.Combine(webHostEnvironment.ContentRootPath, section);
        _baseUrl = baseUrl;
    }

    /// <summary>
    /// Сохраняет изображение под именем <paramref name="name"/> с расширением исходного файла.
    /// </summary>
    [HttpPost("{year:int}/{month:int}/{name}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult> UploadImageAsync(IFormFile formFile, int year, int month, string name, CancellationToken cancellationToken = default)
    {
        string fileExtension = Path.GetExtension(formFile.FileName).ToLowerInvariant();

        if (!ContentTypesByExtension.ContainsKey(fileExtension))
        {
            return Problem(
                title: "Wrong image format",
                detail: $"Image {formFile.FileName} has wrong format.{Environment.NewLine}Available formats: {string.Join(", ", ContentTypesByExtension.Keys)}",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (formFile.Length == 0)
        {
            return Problem(
                title: "Empty file",
                detail: "Form file length is 0",
                statusCode: StatusCodes.Status400BadRequest);
        }

        try
        {
            string yearMonthPath = GetDirectoryPath(year, month);
            Directory.CreateDirectory(yearMonthPath);

            string fileName = $"{Path.GetFileNameWithoutExtension(name)}{fileExtension}";
            string fullPath = Path.Combine(yearMonthPath, fileName);

            await using (FileStream fileStream = new(fullPath, FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream, cancellationToken);
            }

            string imageUrl = $"{_baseUrl}/{year}/{month}/{fileName}";
            return Created(imageUrl, new { fileName, size = formFile.Length, url = imageUrl });
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Error saving image",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Отдаёт изображение.
    /// </summary>
    [HttpGet("{year:int}/{month:int}/{imagePath}")]
    public IActionResult GetImage(int year, int month, string imagePath)
    {
        try
        {
            string fullPath = Path.Combine(GetDirectoryPath(year, month), Path.GetFileName(imagePath));

            if (!System.IO.File.Exists(fullPath))
                return NotFound($"Image {imagePath} not found");

            if (!ContentTypesByExtension.TryGetValue(Path.GetExtension(fullPath).ToLowerInvariant(), out string? contentType))
                return BadRequest("Unsupported image format");

            FileStream fileStream = new(fullPath, FileMode.Open, FileAccess.Read);
            return File(fileStream, contentType, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Error retrieving image",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Имена изображений за месяц.
    /// </summary>
    [HttpGet("{year:int}/{month:int}")]
    public ActionResult<List<string>> GetImages(int year, int month)
    {
        try
        {
            string directoryPath = GetDirectoryPath(year, month);

            if (!Directory.Exists(directoryPath))
                return new List<string>();

            List<string> images = Directory.GetFiles(directoryPath)
                .Select(Path.GetFileName)
                .OfType<string>()
                .ToList();

            return Ok(images);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Error listing images",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private string GetDirectoryPath(int year, int month)
    {
        return Path.Combine(_baseImagesPath, year.ToString(), month.ToString());
    }
}
