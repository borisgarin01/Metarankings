using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games.Developers;
using ExcelProcessors;

namespace API.Controllers.Games;

[Route("api/games/[controller]")]
public sealed class DevelopersController : CrudControllerBase<Developer, AddDeveloperModel, UpdateDeveloperModel>
{
    private readonly IDevelopersRepository _developersRepository;
    private readonly IExcelDataReader<AddDeveloperModel> _developersExcelDataReader;

    public DevelopersController(IDevelopersRepository developersRepository, IExcelDataReader<AddDeveloperModel> developersExcelDataReader) : base(developersRepository)
    {
        _developersRepository = developersRepository;
        _developersExcelDataReader = developersExcelDataReader;
    }

    [HttpPost("upload-developers-from-json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public Task<ActionResult> AddFromJsonAsync(IEnumerable<AddDeveloperModel> developers, CancellationToken cancellationToken = default)
    {
        return AddRangeFromJsonAsync(developers, "Developers", cancellationToken);
    }

    [HttpPost("developers-excel-upload")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public Task<ActionResult> AddFromExcelAsync(IFormFile excelFileWithDevelopers, CancellationToken cancellationToken = default)
    {
        return AddRangeFromExcelAsync(excelFileWithDevelopers, _developersExcelDataReader.GetFromExcel, cancellationToken);
    }

    protected override async Task<ActionResult?> ValidateAddAsync(AddDeveloperModel addDeveloperModel, CancellationToken cancellationToken = default)
    {
        Developer? existingDeveloper = await _developersRepository.GetByNameAsync(addDeveloperModel.Name, cancellationToken);

        if (existingDeveloper is not null)
            return BadRequest($"Разраб с именем {existingDeveloper.Name} уже существует");

        return null;
    }
}
