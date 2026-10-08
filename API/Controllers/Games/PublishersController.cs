using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.Publishers;
using ExcelProcessors;

namespace API.Controllers.Games;

[Route("api/games/[controller]")]
public sealed class PublishersController : CrudControllerBase<Publisher, AddPublisherModel, UpdatePublisherModel>
{
    private readonly IExcelDataReader<AddPublisherModel> _publishersExcelDataReader;

    public PublishersController(IRepository<Publisher, AddPublisherModel, UpdatePublisherModel> publishersRepository, IExcelDataReader<AddPublisherModel> publishersExcelDataReader) : base(publishersRepository)
    {
        _publishersExcelDataReader = publishersExcelDataReader;
    }

    [HttpPost("publishers-excel-upload")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public Task<ActionResult> AddFromExcelAsync(IFormFile excelFileWithPublishers, CancellationToken cancellationToken = default)
    {
        return AddRangeFromExcelAsync(excelFileWithPublishers, _publishersExcelDataReader.GetFromExcel, cancellationToken);
    }

    [HttpPost("upload-publishers-from-json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public Task<ActionResult> AddFromJsonAsync(IEnumerable<AddPublisherModel> publishers, CancellationToken cancellationToken = default)
    {
        return AddRangeFromJsonAsync(publishers, "Publishers", cancellationToken);
    }
}
