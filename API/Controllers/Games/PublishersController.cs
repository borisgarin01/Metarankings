using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games.Publishers;
using ExcelProcessors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace API.Controllers.Games;

[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Games })]
[Route("api/games/[controller]")]
public sealed class PublishersController : CrudControllerBase<Publisher, AddPublisherModel, UpdatePublisherModel>
{
    private readonly PublishersRepository _publishersRepository;
    private readonly IExcelDataReader<AddPublisherModel> _publishersExcelDataReader;

    public PublishersController(PublishersRepository publishersRepository, IExcelDataReader<AddPublisherModel> publishersExcelDataReader) : base(publishersRepository)
    {
        _publishersRepository = publishersRepository;
        _publishersExcelDataReader = publishersExcelDataReader;
    }

    [HttpPost("upload-publishers-from-json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public Task<ActionResult> AddFromJsonAsync(IEnumerable<AddPublisherModel> publishers)
    {
        return AddRangeFromJsonAsync(publishers, "Publishers");
    }

    [HttpPost("publishers-excel-upload")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public Task<ActionResult> AddFromExcelAsync(IFormFile excelFileWithPublishers)
    {
        return AddRangeFromExcelAsync(excelFileWithPublishers, _publishersExcelDataReader.GetFromExcel);
    }

    protected override async Task<ActionResult?> ValidateAddAsync(AddPublisherModel addPublisherModel)
    {
        Publisher? existingPublisher = await _publishersRepository.GetByNameAsync(addPublisherModel.Name);

        if (existingPublisher is not null)
            return BadRequest($"Издатель с именем {existingPublisher.Name} уже существует");

        return null;
    }
}