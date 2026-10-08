using Data.Repositories.Interfaces;

namespace API.Controllers;

/// <summary>
/// Базовый контроллер справочника со стандартным набором CRUD-эндпоинтов.
/// Чтение доступно всем, изменение — только администраторам.
/// Валидацию модели выполняет [ApiController], необработанные исключения — middleware.
/// </summary>
[ApiController]
public abstract class CrudControllerBase<T, TAdd, TUpdate> : ControllerBase
    where T : class
{
    /// <summary>
    /// Репозиторий сущностей контроллера.
    /// </summary>
    protected IRepository<T, TAdd, TUpdate> Repository { get; }

    /// <summary>
    /// Создаёт контроллер поверх репозитория.
    /// </summary>
    protected CrudControllerBase(IRepository<T, TAdd, TUpdate> repository)
    {
        Repository = repository;
    }

    /// <summary>
    /// Все сущности.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<T>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Ok(await Repository.GetAllAsync(cancellationToken));
    }

    /// <summary>
    /// Страница сущностей.
    /// </summary>
    [HttpGet("{offset:long}/{limit:long}")]
    public async Task<ActionResult<IEnumerable<T>>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        return Ok(await Repository.GetAsync(offset, limit, cancellationToken));
    }

    /// <summary>
    /// Сущность по идентификатору.
    /// </summary>
    [HttpGet("{id:long}")]
    public virtual async Task<ActionResult<T>> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        T? entity = await Repository.GetAsync(id, cancellationToken);
        if (entity is null)
            return NotFound();

        return Ok(entity);
    }

    /// <summary>
    /// Добавляет сущность и возвращает её.
    /// </summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<T>> AddAsync(TAdd addModel, CancellationToken cancellationToken = default)
    {
        ActionResult? validationError = await ValidateAddAsync(addModel, cancellationToken);
        if (validationError is not null)
            return validationError;

        long insertedId = await Repository.AddAsync(addModel, cancellationToken);

        T insertedEntity = await Repository.GetAsync(insertedId, cancellationToken);

        return Created($"{Request.Path}/{insertedId}", insertedEntity);
    }

    /// <summary>
    /// Обновляет сущность и возвращает её новое состояние.
    /// </summary>
    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<T>> UpdateAsync(long id, TUpdate updateModel, CancellationToken cancellationToken = default)
    {
        T? entityToUpdate = await Repository.GetAsync(id, cancellationToken);
        if (entityToUpdate is null)
            return NotFound();

        return Ok(await Repository.UpdateAsync(updateModel, id, cancellationToken));
    }

    /// <summary>
    /// Удаляет сущность.
    /// </summary>
    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        T? entity = await Repository.GetAsync(id, cancellationToken);
        if (entity is null)
            return NotFound();

        await Repository.RemoveAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Дополнительная проверка перед добавлением; null — проверка пройдена.
    /// </summary>
    protected virtual Task<ActionResult?> ValidateAddAsync(TAdd addModel, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ActionResult?>(null);
    }

    /// <summary>
    /// Массовое добавление из JSON-массива.
    /// </summary>
    protected async Task<ActionResult> AddRangeFromJsonAsync(IEnumerable<TAdd>? addModels, string entitiesName, CancellationToken cancellationToken = default)
    {
        if (addModels is null)
            return Problem($"{entitiesName} don't set", null, 400);

        if (!addModels.Any())
            return Problem($"{entitiesName} array is empty", null, 400);

        await Repository.AddRangeAsync(addModels, cancellationToken);

        return Ok();
    }

    /// <summary>
    /// Массовое добавление из загруженного Excel-файла (.xlsx).
    /// </summary>
    protected async Task<ActionResult> AddRangeFromExcelAsync(IFormFile? excelFile, Func<string, IEnumerable<TAdd>> readExcelFile, CancellationToken cancellationToken = default)
    {
        if (excelFile is null)
            return Problem("File hasn't set", null, 400);

        if (!excelFile.FileName.EndsWith(".xlsx"))
            return Problem("This is not an Excel file", null, 400);

        string uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(uploadsFolderPath);

        string filePath = Path.Combine(uploadsFolderPath, Path.GetFileName(excelFile.FileName));

        await using (FileStream fileStream = new(filePath, FileMode.Create))
        {
            await excelFile.CopyToAsync(fileStream, cancellationToken);
        }

        try
        {
            await Repository.AddRangeAsync(readExcelFile(filePath), cancellationToken);
        }
        finally
        {
            System.IO.File.Delete(filePath);
        }

        return Ok();
    }
}
