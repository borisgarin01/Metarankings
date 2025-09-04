using IdentityLibrary.Telegram;
using Microsoft.AspNetCore.Http;

namespace API.Controllers.Games;

[ApiController]
[Route("api/games/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    private readonly TelegramAuthenticator _telegramAuthenticator;

    public ImagesController(IWebHostEnvironment webHostEnvironment, TelegramAuthenticator telegramAuthenticator)
    {
        _webHostEnvironment = webHostEnvironment;
        _telegramAuthenticator = telegramAuthenticator;
    }

    [HttpPost("{year:int}/{month:int}/{imagePath}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult> UploadImageAsync(IFormFile formFile, int? year, int? month, string imagePath)
    {
        //JPEG, PNG, GIF, TIFF, WebP, SVG, BMP, and HEIF

        if (!formFile.FileName.EndsWith(".jpg")
            && !formFile.FileName.EndsWith(".png")
            && !formFile.FileName.EndsWith(".svg")
            && !formFile.FileName.EndsWith(".jpeg")
            && !formFile.FileName.EndsWith(".gif")
            && !formFile.FileName.EndsWith(".tiff")
            && !formFile.FileName.EndsWith(".webp")
            && !formFile.FileName.EndsWith(".bmp")
            && !formFile.FileName.EndsWith(".heif"))

            return Problem(title: "Wrong image format",
            detail: $"Image {formFile.FileName} has wrong format.{Environment.NewLine}Available formats: .jpg, .png, .svg, .jpeg, .gif, .tiff, .webp, .bmp, .heif",
            statusCode: StatusCodes.Status400BadRequest);

        if (string.IsNullOrWhiteSpace(imagePath))
            return Problem(
                title: "Image path is null or white space",
                detail: "Image path is null or white space",
                statusCode: StatusCodes.Status422UnprocessableEntity
                );

        if (formFile.Length > 0)
        {
            string pathToImage;
            if (year is not null && month is not null)
                pathToImage = $"{_webHostEnvironment.ContentRootPath}/Games/Images/Uploads/{year}/{month}/{formFile.FileName}";
            else
                pathToImage = $"{_webHostEnvironment.ContentRootPath}/Games/Images/Uploads/{formFile.FileName}";

            string imageFolder = Path.GetDirectoryName(pathToImage);

            if (!Directory.Exists(imageFolder))
                Directory.CreateDirectory(imageFolder);

            using (var fileStream = new FileStream($"{imageFolder}/{formFile.FileName}", FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            }

            return Created($"api/images/{formFile.FileName}", formFile);
        }

        return Problem(title: "Form file length == 0", detail: "Form file length == 0", statusCode: StatusCodes.Status400BadRequest);
    }

    [HttpGet("{year:int}/{month:int}/{imagePath}")]
    public IActionResult GetImage(int year, int month, string imagePath)
    {
        string pathToImage = $@"{_webHostEnvironment.ContentRootPath}\Games\Images\Uploads\{year}\{month}\{imagePath}";

        if (!System.IO.File.Exists(pathToImage))
            return Problem(
                title: "Image doesn't exist",
                detail: $"Image {pathToImage} doesn't exist",
                statusCode: StatusCodes.Status500InternalServerError);

        if (pathToImage.EndsWith(".jpeg"))
            return File(System.IO.File.ReadAllBytes(pathToImage), "image/jpeg");
        if (pathToImage.EndsWith(".jpg"))
            return File(System.IO.File.ReadAllBytes(pathToImage), "image/jpg");
        if (pathToImage.EndsWith(".png"))
            return File(System.IO.File.ReadAllBytes(pathToImage), "image/png");
        if (pathToImage.EndsWith(".svg"))
            return File(System.IO.File.ReadAllBytes(pathToImage), "image/svg");
        if (pathToImage.EndsWith(".gif"))
            return File(System.IO.File.ReadAllBytes(pathToImage), "image/gif");
        if (pathToImage.EndsWith(".tiff"))
            return File(System.IO.File.ReadAllBytes(pathToImage), "image/tiff");
        if (pathToImage.EndsWith(".webp"))
            return File(System.IO.File.ReadAllBytes(pathToImage), "image/webp");
        if (pathToImage.EndsWith(".bmp"))
            return File(System.IO.File.ReadAllBytes(pathToImage), "image/bmp");
        if (pathToImage.EndsWith(".heif"))
            return File(System.IO.File.ReadAllBytes(pathToImage), "image/heif");

        return BadRequest("Unknown image format");
    }
}
