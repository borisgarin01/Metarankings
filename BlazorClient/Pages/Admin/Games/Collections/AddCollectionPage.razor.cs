using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Games.Collections;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using WebManagers;

namespace BlazorClient.Pages.Admin.Games.Collections;

public partial class AddCollectionPage : ComponentBase
{
    [Inject]
    public IWebManager<Game, AddGameModel, UpdateGameModel> GamesWebManager { get; set; }

    [Inject]
    public IWebManager<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel> GamesCollectionsWebManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string CollectionName { get; set; }

    [Required]
    [MaxLength(4000)]
    [MinLength(1)]
    public string Description { get; set; }

    public IEnumerable<Game> GamesToSelectFrom { get; set; }

    public IEnumerable<long> SelectedGamesIds { get; private set; }
    public IBrowserFile ImageToUpload { get; private set; }
    public string ImageSource { get; set; }

    const int MAX_FILESIZE = 5000 * 1024;

    protected override async Task OnInitializedAsync()
    {
        GamesToSelectFrom = await GamesWebManager.GetAllAsync();
    }

    public Task SelectGames(ChangeEventArgs e)
    {
        SelectedGamesIds = ((string[])e.Value)
            .Select(idString => long.Parse(idString))
            .ToList();
        return Task.CompletedTask;
    }

    public async Task AddGameCollectionAsync()
    {
        var addGameCollectionModel = new AddGamesCollectionModel(CollectionName, Description, ImageSource, SelectedGamesIds);

        HttpResponseMessage gameCreationHttpResponseMessage = await GamesCollectionsWebManager.AddAsync(addGameCollectionModel);

        if (!gameCreationHttpResponseMessage.IsSuccessStatusCode)
            await JSRuntime.InvokeVoidAsync("alert", await gameCreationHttpResponseMessage.Content.ReadAsStringAsync());

        else
            NavigationManager.NavigateTo("/games/collections");
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
