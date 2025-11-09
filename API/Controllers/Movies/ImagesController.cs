using IdentityLibrary.Telegram;
using SixLabors.ImageSharp;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/movies/[controller]")]
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
        _baseImagesPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Movies");
    }

    [HttpPost("{year:int}/{month:int}/{imageName}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult> UploadImageAsync(IFormFile formFile, int? year, int? month, string imageName)
    {
        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg", ".gif", ".tiff", ".webp", ".bmp", ".heif" };
        var fileExtension = Path.GetExtension(formFile.FileName).ToLowerInvariant();

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
            var yearMonthPath = Path.Combine(_baseImagesPath, year.ToString(), month.ToString());
            if (!Directory.Exists(yearMonthPath))
            {
                Directory.CreateDirectory(yearMonthPath);
            }

            // Use the provided imageName but keep the original extension
            var fileName = $"{Path.GetFileNameWithoutExtension(imageName)}{fileExtension}";
            var fullPath = Path.Combine(yearMonthPath, fileName);

            // Save the file
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            }

            // Return the URL to access the image
            var imageUrl = Url.Action("GetImage", new { year, month, imagePath = fileName });
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
        if (!System.IO.File.Exists($"{_webHostEnvironment.WebRootPath}{Path.DirectorySeparatorChar}Movies{Path.DirectorySeparatorChar}Images{Path.DirectorySeparatorChar}{year}{Path.DirectorySeparatorChar}{month}{Path.DirectorySeparatorChar}{imagePath}"))
            return Problem(
                title: "Image doesn't exist",
                detail: $"Image {_webHostEnvironment.WebRootPath}/Movies/Images/{imagePath} doesn't exist",
                statusCode: StatusCodes.Status500InternalServerError);

        return File($"{_webHostEnvironment.WebRootPath}/Movies/Images/{imagePath}", imagePath.Substring(imagePath.IndexOf("."), imagePath.Length - imagePath.IndexOf(".")));
    }
}
