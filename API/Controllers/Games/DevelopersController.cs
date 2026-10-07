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
    public Task<ActionResult> AddFromJsonAsync(IEnumerable<AddDeveloperModel> developers)
    {
        return AddRangeFromJsonAsync(developers, "Developers");
    }

    [HttpPost("developers-excel-upload")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public Task<ActionResult> AddFromExcelAsync(IFormFile excelFileWithDevelopers)
    {
        return AddRangeFromExcelAsync(excelFileWithDevelopers, _developersExcelDataReader.GetFromExcel);
    }

    protected override async Task<ActionResult?> ValidateAddAsync(AddDeveloperModel addDeveloperModel)
    {
        Developer? existingDeveloper = await _developersRepository.GetByNameAsync(addDeveloperModel.Name);

        if (existingDeveloper is not null)
            return BadRequest($"Разраб с именем {existingDeveloper.Name} уже существует");

        return null;
    }
}
