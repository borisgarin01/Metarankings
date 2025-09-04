using IdentityLibrary.Telegram;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/movies/[controller]")]
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
                pathToImage = $"{_webHostEnvironment.ContentRootPath}/Movies/Images/Uploads/{year}/{month}/{formFile.FileName}";
            else
                pathToImage = $"{_webHostEnvironment.ContentRootPath}/Movies/Images/Uploads/{formFile.FileName}";

            string imageFolder = Path.GetDirectoryName(pathToImage);

            if (!Directory.Exists(imageFolder))
                Directory.CreateDirectory(imageFolder);

            using (var fileStream = new FileStream($"{imageFolder}/{formFile.FileName}", FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            }

            return Created($"api/Movies/images/{formFile.FileName}", formFile);
        }

        return Problem(title: "Form file length == 0", detail: "Form file length == 0", statusCode: StatusCodes.Status400BadRequest);
    }

    [HttpGet("{year:int}/{month:int}/{imagePath}")]
    public IActionResult GetImage(int year, int month, string imagePath)
    {
        if (!System.IO.File.Exists($"{_webHostEnvironment.ContentRootPath}/Movies/Images/Uploads/{year}/{month}/{imagePath}"))
            return Problem(
                title: "Image doesn't exist",
                detail: $"Image {_webHostEnvironment.ContentRootPath}/Movies/Images/{imagePath} doesn't exist",
                statusCode: StatusCodes.Status500InternalServerError);

        return File($"{_webHostEnvironment.ContentRootPath}/Movies/Images/{imagePath}", imagePath.Substring(imagePath.IndexOf("."), imagePath.Length - imagePath.IndexOf(".")));
    }
}
