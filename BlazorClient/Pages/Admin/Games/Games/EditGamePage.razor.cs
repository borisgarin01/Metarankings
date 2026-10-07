using Blazored.Toast.Services;
using Domain.Games;
using Domain.RequestsModels.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using WebManagers.Derived.Games;

namespace BlazorClient.Pages.Admin.Games.Games;

[Authorize(Policy = "Admin")]
public partial class EditGamePage : ComponentBase
{
    const int MAX_FILESIZE = 5000 * 1024;

    [Parameter]
    public long Id { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    [Inject]
    public GamesWebManager GamesWebManager { get; private set; }

    [Inject]
    public NavigationManager NavigationManager { get; private set; }

    public Game? Game { get; private set; }
    public bool IsNotFound { get; private set; }
    private bool IsSaving { get; set; }

    public string Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string Description { get; set; }
    public string? Trailer { get; set; }

    public IEnumerable<Developer> DevelopersToSelectFrom { get; private set; }
    public IEnumerable<Genre> GenresToSelectFrom { get; private set; }
    public IEnumerable<Localization> LocalizationsToSelectFrom { get; private set; }
    public IEnumerable<Platform> PlatformsToSelectFrom { get; private set; }
    public IEnumerable<Publisher> PublishersToSelectFrom { get; private set; }

    public List<long> SelectedDevelopersIds { get; private set; } = new List<long>();
    public List<long> SelectedGenresIds { get; private set; } = new List<long>();
    public long? SelectedLocalizationId { get; private set; }
    public List<long> SelectedPlatformsIds { get; private set; } = new List<long>();
    public List<long> SelectedPublishersIds { get; private set; } = new List<long>();

    /// <summary>Data-url новой обложки или текущая обложка игры.</summary>
    public string ImagePreview { get; private set; }
    public IBrowserFile? ImageToUpload { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        HttpClient httpClient = HttpClientFactory.CreateClient("AuthorizedClient");

        Task<IEnumerable<Developer>> developersGettingTask = httpClient.GetFromJsonAsync<IEnumerable<Developer>>("/api/Games/Developers");
        Task<IEnumerable<Genre>> genresGettingTask = httpClient.GetFromJsonAsync<IEnumerable<Genre>>("/api/Games/Genres");
        Task<IEnumerable<Localization>> localizationsGettingTask = httpClient.GetFromJsonAsync<IEnumerable<Localization>>("/api/Games/Localizations");
        Task<IEnumerable<Platform>> platformsGettingTask = httpClient.GetFromJsonAsync<IEnumerable<Platform>>("/api/Games/Platforms");
        Task<IEnumerable<Publisher>> publishersGettingTask = httpClient.GetFromJsonAsync<IEnumerable<Publisher>>("/api/Games/Publishers");

        try
        {
            Game = await GamesWebManager.GetAsync(Id);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Game = null;
        }

        await Task.WhenAll(developersGettingTask, genresGettingTask, localizationsGettingTask, platformsGettingTask, publishersGettingTask);

        DevelopersToSelectFrom = developersGettingTask.Result;
        GenresToSelectFrom = genresGettingTask.Result;
        LocalizationsToSelectFrom = localizationsGettingTask.Result;
        PlatformsToSelectFrom = platformsGettingTask.Result;
        PublishersToSelectFrom = publishersGettingTask.Result;

        if (Game is null)
        {
            IsNotFound = true;
            return;
        }

        Name = Game.Name;
        ReleaseDate = Game.ReleaseDate?.ToDateTime(TimeOnly.MinValue);
        Description = Game.Description;
        Trailer = Game.Trailer;
        ImagePreview = Game.Image;

        SelectedDevelopersIds = Game.Developers.Select(d => d.Id).ToList();
        SelectedGenresIds = Game.Genres.Select(g => g.Id).ToList();
        SelectedPlatformsIds = Game.Platforms.Select(p => p.Id).ToList();
        SelectedPublishersIds = Game.Publishers.Select(p => p.Id).ToList();
        SelectedLocalizationId = Game.Localization?.Id ?? (Game.LocalizationId > 0 ? Game.LocalizationId : null);
    }

    private async Task FileUploaded(InputFileChangeEventArgs e)
    {
        ImageToUpload = e.File;
        using Stream imageToUploadReadStream = ImageToUpload.OpenReadStream(MAX_FILESIZE);
        using MemoryStream memoryStream = new MemoryStream();
        await imageToUploadReadStream.CopyToAsync(memoryStream);
        ImagePreview = $"data:{ImageToUpload.ContentType};base64,{Convert.ToBase64String(memoryStream.ToArray())}";
    }

    private string? ValidationError()
    {
        if (string.IsNullOrWhiteSpace(Name)) return "Укажите название";
        if (!ReleaseDate.HasValue) return "Укажите дату выхода";
        if (!SelectedDevelopersIds.Any()) return "Выберите разработчиков";
        if (!SelectedGenresIds.Any()) return "Выберите жанры";
        if (!SelectedLocalizationId.HasValue) return "Выберите локализацию";
        if (!SelectedPlatformsIds.Any()) return "Выберите платформы";
        if (!SelectedPublishersIds.Any()) return "Выберите издателей";
        return null;
    }

    private async Task UpdateGameAsync()
    {
        string? validationError = ValidationError();
        if (validationError is not null)
        {
            ToastService.ShowWarning(validationError);
            return;
        }

        IsSaving = true;
        try
        {
            string image = Game!.Image;

            if (ImageToUpload is not null)
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                StreamContent fileContent = new StreamContent(ImageToUpload.OpenReadStream(50 * 1024 * 1024)); // 50MB max
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(ImageToUpload.ContentType);
                content.Add(fileContent, "formFile", ImageToUpload.Name);

                string uploadingImageName = Uri.EscapeDataString(Path.GetRandomFileName());
                string uploadingFileNameWithCorrectExtention = Path.ChangeExtension(uploadingImageName, Path.GetExtension(ImageToUpload.Name));
                string url = $"/api/games/images/{ReleaseDate!.Value.Year}/{ReleaseDate.Value.Month}/{uploadingFileNameWithCorrectExtention}";

                HttpResponseMessage response = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    ToastService.ShowError(await response.Content.ReadAsStringAsync());
                    return;
                }

                image = url;
            }

            UpdateGameModel updateGameModel = new UpdateGameModel(
                Name: Name,
                Image: image,
                DevelopersIds: SelectedDevelopersIds,
                PublishersIds: SelectedPublishersIds,
                GenresIds: SelectedGenresIds,
                LocalizationId: SelectedLocalizationId!.Value,
                ReleaseDate: ReleaseDate,
                Description: Description,
                Trailer: Trailer,
                PlatformsIds: SelectedPlatformsIds);

            Game? updatedGame = await GamesWebManager.UpdateAsync(Id, updateGameModel);

            if (updatedGame is null)
            {
                ToastService.ShowError("Не удалось сохранить игру");
                return;
            }

            ToastService.ShowSuccess("Игра сохранена");
            NavigationManager.NavigateTo("/admin/games/games/list-games");
        }
        catch (Exception ex)
        {
            ToastService.ShowError(ex.Message);
        }
        finally
        {
            IsSaving = false;
        }
    }
}
