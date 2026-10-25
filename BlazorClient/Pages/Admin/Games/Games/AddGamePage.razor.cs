using Blazored.Toast.Services;
using Domain.Common;
using Domain.Games;
using Domain.RequestsModels.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using WebManagers.Derived.Games;

namespace BlazorClient.Pages.Admin.Games.Games;

[Authorize(Policy = "Admin")]
public partial class AddGamePage : ComponentBase
{
    const int MAX_FILESIZE = 5000 * 1024;

    [Inject] public IHttpClientFactory HttpClientFactory { get; set; }
    [Inject] public IToastService ToastService { get; set; }
    [Inject] public GamesWebManager GetWebManager { get; private set; }
    [Inject] public NavigationManager NavigationManager { get; private set; }

    public string Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string Description { get; set; }
    public string? Trailer { get; set; }

    public IEnumerable<Developer> DevelopersToSelectFrom { get; private set; }
    public IEnumerable<Genre> GenresToSelectFrom { get; private set; }
    public IEnumerable<Localization> LocalizationsToSelectFrom { get; private set; }
    public IEnumerable<Platform> PlatformsToSelectFrom { get; private set; }
    public IEnumerable<Publisher> PublishersToSelectFrom { get; private set; }

    // Упорядоченные списки выбранных элементов — порядок в списке = Position.
    public IReadOnlyList<Developer> SelectedDevelopers { get; set; } = Array.Empty<Developer>();
    public IReadOnlyList<Genre> SelectedGenres { get; set; } = Array.Empty<Genre>();
    public IReadOnlyList<Platform> SelectedPlatforms { get; set; } = Array.Empty<Platform>();
    public IReadOnlyList<Publisher> SelectedPublishers { get; set; } = Array.Empty<Publisher>();

    public long? SelectedLocalizationId { get; private set; }
    public string ImageSource { get; private set; }
    public IBrowserFile ImageToUpload { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        HttpClient httpClient = HttpClientFactory.CreateClient("AuthorizedClient");

        Task<IEnumerable<Developer>> developersGettingTask = httpClient.GetFromJsonAsync<IEnumerable<Developer>>("/api/Games/Developers");
        Task<IEnumerable<Genre>> genresGettingTask = httpClient.GetFromJsonAsync<IEnumerable<Genre>>("/api/Games/Genres");
        Task<IEnumerable<Localization>> localizationsGettingTask = httpClient.GetFromJsonAsync<IEnumerable<Localization>>("/api/Games/Localizations");
        Task<IEnumerable<Platform>> platformsGettingTask = httpClient.GetFromJsonAsync<IEnumerable<Platform>>("/api/Games/Platforms");
        Task<IEnumerable<Publisher>> publishersGettingTask = httpClient.GetFromJsonAsync<IEnumerable<Publisher>>("/api/Games/Publishers");

        await Task.WhenAll(developersGettingTask, genresGettingTask, localizationsGettingTask, platformsGettingTask, publishersGettingTask);

        DevelopersToSelectFrom = developersGettingTask.Result;
        GenresToSelectFrom = genresGettingTask.Result;
        LocalizationsToSelectFrom = localizationsGettingTask.Result;
        PlatformsToSelectFrom = platformsGettingTask.Result;
        PublishersToSelectFrom = publishersGettingTask.Result;
    }

    public async Task AddGameAsync()
    {
        if (!GameModelToAddConfigured())
        {
            ToastService.ShowWarning("Заполните все обязательные поля и выберите обложку.");
            return;
        }

        try
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            StreamContent fileContent = new StreamContent(ImageToUpload.OpenReadStream(50 * 1024 * 1024));
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

            AddGameModel addGameModel = new AddGameModel(
                Name: Name,
                Image: url,
                Developers: ToOrderedIds(SelectedDevelopers),
                Publishers: ToOrderedIds(SelectedPublishers),
                Genres: ToOrderedIds(SelectedGenres),
                LocalizationId: SelectedLocalizationId!.Value,
                ReleaseDate: ReleaseDate,
                Description: Description,
                Trailer: Trailer,
                Platforms: ToOrderedIds(SelectedPlatforms));

            HttpResponseMessage addingGameResponseMessage = await GetWebManager.AddAsync(addGameModel);

            if (addingGameResponseMessage.IsSuccessStatusCode)
                NavigationManager.NavigateTo("/admin/games/games/list-games");
            else
                ToastService.ShowError(await addingGameResponseMessage.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"{ex.Message}\t{ex.StackTrace}");
        }
    }

    private bool GameModelToAddConfigured()
    {
        return SelectedDevelopers.Count > 0
            && SelectedGenres.Count > 0
            && SelectedLocalizationId.HasValue
            && SelectedPlatforms.Count > 0
            && SelectedPublishers.Count > 0
            && ImageToUpload is not null;
    }

    private static IEnumerable<OrderedId> ToOrderedIds<T>(IReadOnlyList<T> items, Func<T, long> idSelector)
        where T : class
        => items.Select((item, index) => new OrderedId(idSelector(item), index)).ToList();

    private IEnumerable<OrderedId> ToOrderedIds(IReadOnlyList<Developer> items)
        => ToOrderedIds(items, d => d.Id);

    private IEnumerable<OrderedId> ToOrderedIds(IReadOnlyList<Publisher> items)
        => ToOrderedIds(items, p => p.Id);

    private IEnumerable<OrderedId> ToOrderedIds(IReadOnlyList<Genre> items)
        => ToOrderedIds(items, g => g.Id);

    private IEnumerable<OrderedId> ToOrderedIds(IReadOnlyList<Platform> items)
        => ToOrderedIds(items, p => p.Id);

    private async Task FileUploaded(InputFileChangeEventArgs e)
    {
        ImageToUpload = e.File;
        using Stream imageToUploadReadStream = ImageToUpload.OpenReadStream(MAX_FILESIZE);
        using MemoryStream memoryStream = new MemoryStream();
        await imageToUploadReadStream.CopyToAsync(memoryStream);
        ImageSource = $"data:{ImageToUpload.ContentType};base64,{Convert.ToBase64String(memoryStream.ToArray())}";
    }
}