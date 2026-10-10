using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.Publishers;
using ExcelProcessors;

namespace API.Controllers.Games;

[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Games })]
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
    public Task<ActionResult> AddFromExcelAsync(IFormFile excelFileWithPublishers)
    {
        return AddRangeFromExcelAsync(excelFileWithPublishers, _publishersExcelDataReader.GetFromExcel);
    }

    [HttpPost("upload-publishers-from-json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public Task<ActionResult> AddFromJsonAsync(IEnumerable<AddPublisherModel> publishers)
    {
        return AddRangeFromJsonAsync(publishers, "Publishers");
    }
}
