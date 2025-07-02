using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Games
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class GamesImagesController : ControllerBase
    {
        [HttpPost("{year}/{month}")]
        public async Task<ActionResult> AddImageAsync(IFormFile formFile, short year, byte month)
        {
            // Validate file extension
            var fileExtension = Path.GetExtension(formFile.FileName).ToLowerInvariant();
            if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(fileExtension))
            {
                return BadRequest("Bad file extension. Accepts: .jpg, .jpeg, .png");
            }

            // Validate file size (e.g., 5MB limit)
            if (formFile.Length > 5 * 1024 * 1024)
            {
                return BadRequest("File size exceeds 5MB limit");
            }

            // Create directory path
            var monthString = month < 10 ? $"0{month}" : month.ToString();
            var uploadPath = Path.Combine("Uploads", "Images", "Games", year.ToString(), monthString);
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);

            // Create directory if it doesn't exist
            Directory.CreateDirectory(fullPath);

            // Generate a unique filename to prevent overwrites
            var filePath = Path.Combine(fullPath, formFile.FileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            // Return the relative path or other response
            return Ok(new { Path = Path.Combine(uploadPath, formFile.FileName) });
        }
    }
}
