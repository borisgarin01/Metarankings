using Blazored.Toast.Services;
using Domain.Games;
using Domain.RequestsModels.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using System.Net.Http;
using WebManagers;
using WebManagers.Derived.Games;

namespace BlazorClient.Pages.Admin.Games.Games;

[Authorize(Policy = "Admin")]
public partial class AddGamePage : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        Task<IEnumerable<Developer>> developersGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Developer>>("/api/Games/Developers");
        Task<IEnumerable<Genre>> genresGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Genre>>("/api/Games/Genres");
        Task<IEnumerable<Localization>> localizationsGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Localization>>("/api/Games/Localizations");
        Task<IEnumerable<Platform>> platformsGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Platform>>("/api/Games/Platforms");
        Task<IEnumerable<Publisher>> publishersGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Publisher>>("/api/Games/Publishers");

        await Task.WhenAll(developersGettingTask, genresGettingTask, localizationsGettingTask, platformsGettingTask, publishersGettingTask);

        DevelopersToSelectFrom = developersGettingTask.Result;
        GenresToSelectFrom = genresGettingTask.Result;
        LocalizationsToSelectFrom = localizationsGettingTask.Result;
        PlatformsToSelectFrom = platformsGettingTask.Result;
        PublishersToSelectFrom = publishersGettingTask.Result;
    }

    const int MAX_FILESIZE = 5000 * 1024;

    [Inject]
    public HttpClient HttpClient { get; private set; }

    [Inject]
    public IToastService ToastService { get; set; }

    [Inject]
    public GamesWebManager GetWebManager { get; private set; }

    [Inject]
    public NavigationManager NavigationManager { get; private set; }

    public string Name { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Description { get; set; }
    public string Trailer { get; set; }
    public IEnumerable<Developer> DevelopersToSelectFrom { get; private set; }
    public IEnumerable<Genre> GenresToSelectFrom { get; private set; }
    public IEnumerable<Localization> LocalizationsToSelectFrom { get; private set; }
    public IEnumerable<Platform> PlatformsToSelectFrom { get; private set; }
    public IEnumerable<Publisher> PublishersToSelectFrom { get; private set; }

    public List<long> SelectedDevelopersIds { get; private set; } = new List<long>();
    public List<long> SelectedGenresIds { get; private set; } = new List<long>();
    public long SelectedLocalizationId { get; private set; }
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
                var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(ImageToUpload.OpenReadStream(50 * 1024 * 1024)); // 50MB max
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(ImageToUpload.ContentType);
                content.Add(fileContent, "formFile", ImageToUpload.Name);

                string uploadingImageName = Uri.EscapeDataString(Path.GetRandomFileName());
                string uploadingFileNameWithCorrectExtention = Path.ChangeExtension(uploadingImageName, Path.GetExtension(ImageToUpload.Name));

                // Build the URL with parameters
                var url = $"api/games/images/{ReleaseDate.Year}/{ReleaseDate.Month}/{uploadingFileNameWithCorrectExtention}";

                // Send the request with authentication token
                var response = await HttpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    // Extract the URL from the response
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var addGameModel = new AddGameModel(Name, url, SelectedDevelopersIds, SelectedPublishersIds, SelectedGenresIds, SelectedLocalizationId, ReleaseDate, Description, Trailer, SelectedPlatformsIds);

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
        return !SelectedDevelopersIds.Contains(-1)
            && !SelectedGenresIds.Contains(-1)
            && SelectedLocalizationId != -1
            && !SelectedPlatformsIds.Contains(-1)
            && !SelectedPublishersIds.Contains(-1)
            && ImageToUpload is not null;
    }

    private async Task FileUploaded(InputFileChangeEventArgs e)
    {
        ImageToUpload = e.File;
        using (Stream imageToUploadReadStream = ImageToUpload.OpenReadStream(MAX_FILESIZE))
        {
            using (var memoryStream = new MemoryStream())
            {
                await imageToUploadReadStream.CopyToAsync(memoryStream);
                ImageSource = $"data:{ImageToUpload.ContentType};base64,{Convert.ToBase64String(memoryStream.ToArray())}";
            }
        }
    }
}
