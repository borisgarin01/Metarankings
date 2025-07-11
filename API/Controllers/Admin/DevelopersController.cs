using Data.Repositories.Interfaces;
using Domain.Games;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers.Admin;

[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/admin/[controller]")]
public sealed class DevelopersController : ControllerBase
{
    private readonly IRepository<Developer> _developersRepository;
    public DevelopersController(IRepository<Developer> developersRepository)
    {
        _developersRepository = developersRepository;
    }

    [HttpPost]
    public async Task<ActionResult> UploadFromExcel(IFormFile formFile)
    {
        if (!formFile.FileName.EndsWith(".xlsx") && !formFile.FileName.EndsWith(".xls"))
            return BadRequest("It's not an Excel file");

        var developers = new List<Developer>();
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
                    developers.Add(new Developer { Name = activeCell.StringCellValue });
                    rowIndex++;
                }
            }
        }

        await _developersRepository.AddRangeAsync(developers);

        return Ok(new { Result = "Uploaded" });
    }
}
