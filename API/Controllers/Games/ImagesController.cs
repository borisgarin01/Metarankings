using IdentityLibrary.Telegram;

namespace API.Controllers.Games;

[ApiController]
[Route("api/games/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly TelegramAuthenticator _telegramAuthenticator;
    private readonly string _baseImagesPath;

    public ImagesController(IWebHostEnvironment webHostEnvironment, TelegramAuthenticator telegramAuthenticator)
    {
        _webHostEnvironment = webHostEnvironment;
        _telegramAuthenticator = telegramAuthenticator;

        // Use a consistent base path for images
        _baseImagesPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Games");
    }

    [HttpPost("{year:int}/{month:int}/{name}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult> UploadImageAsync(IFormFile formFile, int year, int month, string name)
    {
        // Validate file type
        string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg", ".gif", ".tiff", ".webp", ".bmp", ".heif" };
        string fileExtension = Path.GetExtension(formFile.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            return Problem(
                title: "Wrong image format",
                detail: $"Image {formFile.FileName} has wrong format.{Environment.NewLine}Available formats: {string.Join(", ", allowedExtensions)}",
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
            // Create directory structure
            string yearMonthPath = Path.Combine(_baseImagesPath, year.ToString(), month.ToString());
            if (!Directory.Exists(yearMonthPath))
            {
                _ = Directory.CreateDirectory(yearMonthPath);
            }

            // Use the provided imageName but keep the original extension
            string fileName = $"{Path.GetFileNameWithoutExtension(name)}{fileExtension}";
            string fullPath = Path.Combine(yearMonthPath, fileName);

            // Save the file
            using (FileStream fileStream = new(fullPath, FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            }

            // Return the URL to access the image
            string imageUrl = $"/api/movies/Images/{year}/{month}/{fileName}";
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

    [HttpGet("{year:int}/{month:int}/{imagePath}")]
    public IActionResult GetImage(int year, int month, string imagePath)
    {
        try
        {
            string fullPath = Path.Combine(_baseImagesPath, year.ToString(), month.ToString(), imagePath);

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound($"Image {imagePath} not found");
            }

            // Get content type based on file extension
            string contentType = GetContentType(fullPath);
            if (contentType == null)
            {
                return BadRequest("Unsupported image format");
            }

            // Use FileStreamResult for better performance with large files
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

    private string GetContentType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".svg" => "image/svg+xml",
            ".gif" => "image/gif",
            ".tiff" => "image/tiff",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".heif" => "image/heif",
            _ => null
        };
    }

    // Optional: Add method to list images
    [HttpGet("{year:int}/{month:int}")]
    public ActionResult<List<string>> GetImages(int year, int month)
    {
        try
        {
            string directoryPath = Path.Combine(_baseImagesPath, year.ToString(), month.ToString());

            if (!Directory.Exists(directoryPath))
            {
                return new List<string>();
            }

            List<string?> images = Directory.GetFiles(directoryPath)
                .Select(Path.GetFileName)
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
}