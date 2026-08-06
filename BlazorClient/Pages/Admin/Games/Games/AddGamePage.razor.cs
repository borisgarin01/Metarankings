using Blazored.Toast.Services;
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

    const int MAX_FILESIZE = 5000 * 1024;

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    [Inject]
    public GamesWebManager GetWebManager { get; private set; }

    [Inject]
    public NavigationManager NavigationManager { get; private set; }

    public string Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string Description { get; set; }
    public string Trailer { get; set; }
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
    public string ImageSource { get; private set; }
    public IBrowserFile ImageToUpload { get; private set; }
    public async Task AddGameAsync()
    {
        if (GameModelToAddConfigured())
        {
            try
            {
                // Create multipart form data
                MultipartFormDataContent content = new MultipartFormDataContent();
                StreamContent fileContent = new StreamContent(ImageToUpload.OpenReadStream(50 * 1024 * 1024)); // 50MB max
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(ImageToUpload.ContentType);
                content.Add(fileContent, "formFile", ImageToUpload.Name);

                string uploadingImageName = Uri.EscapeDataString(Path.GetRandomFileName());
                string uploadingFileNameWithCorrectExtention = Path.ChangeExtension(uploadingImageName, Path.GetExtension(ImageToUpload.Name));

                // Build the URL with parameters
                string url = $"/api/games/images/{ReleaseDate.Value.Year}/{ReleaseDate.Value.Month}/{uploadingFileNameWithCorrectExtention}";

                // Send the request with authentication token
                HttpResponseMessage response = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    // Extract the URL from the response
                    string responseContent = await response.Content.ReadAsStringAsync();

                    AddGameModel addGameModel = new AddGameModel(Name, url, SelectedDevelopersIds, SelectedPublishersIds, SelectedGenresIds, SelectedLocalizationId.Value, ReleaseDate, Description, Trailer, SelectedPlatformsIds);

                    HttpResponseMessage addingGameResponseMessage = await GetWebManager.AddAsync(addGameModel);

                    if (addingGameResponseMessage.IsSuccessStatusCode)
                        NavigationManager.NavigateTo("/admin/games/games/list-games");
                }
                else
                    ToastService.ShowError(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"{ex.Message}\t{ex.StackTrace}");
            }
        }
    }

    private bool GameModelToAddConfigured()
    {
        return SelectedDevelopersIds.Any()
        && SelectedGenresIds.Any()
        && SelectedLocalizationId.HasValue // Проверяем что выбран
        && SelectedPlatformsIds.Any()
        && SelectedPublishersIds.Any()
        && ImageToUpload is not null;
    }

    private async Task FileUploaded(InputFileChangeEventArgs e)
    {
        ImageToUpload = e.File;
        using Stream imageToUploadReadStream = ImageToUpload.OpenReadStream(MAX_FILESIZE);
        using MemoryStream memoryStream = new MemoryStream();
        await imageToUploadReadStream.CopyToAsync(memoryStream);
        ImageSource = $"data:{ImageToUpload.ContentType};base64,{Convert.ToBase64String(memoryStream.ToArray())}";
    }
}
