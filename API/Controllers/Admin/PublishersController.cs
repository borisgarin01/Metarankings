using Data.Repositories.Interfaces;
using Domain.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace API.Controllers.Admin;

[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/admin/[controller]")]
public sealed class PublishersController : ControllerBase
{
    private readonly IRepository<Publisher> _publishersRepository;

    public PublishersController(IRepository<Publisher> publishersRepository)
    {
        _publishersRepository = publishersRepository;
    }

    [HttpPost]
    public async Task<ActionResult> UploadFromExcel(IFormFile formFile)
    {
        if (!formFile.FileName.EndsWith(".xlsx") && !formFile.FileName.EndsWith(".xls"))
            return BadRequest("It's not an Excel file");

        var publishers = new List<Publisher>();
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        using (var stream = new MemoryStream())
        {
            formFile.CopyTo(stream);
            stream.Position = 0;

            using (IWorkbook workbook = formFile.FileName.EndsWith(".xlsx") ? new XSSFWorkbook(stream) : new HSSFWorkbook(stream))
            {
                int sheetIndex = 0;
                var sheet = workbook.GetSheetAt(sheetIndex);
                int rowIndex = 0;
                int columnIndex = 0;
                ICell activeCell;
                while (sheet.GetRow(rowIndex) != null)
                {
                    activeCell = sheet.GetRow(rowIndex).GetCell(columnIndex);
                    publishers.Add(new Publisher { Name = activeCell.StringCellValue });
                    rowIndex++;
                }
            }
        }

        await _publishersRepository.AddRangeAsync(publishers);

        return Ok(new { Result = "Uploaded" });
    }
}
