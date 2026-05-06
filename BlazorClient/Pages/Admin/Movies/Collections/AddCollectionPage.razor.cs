using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.Collections;

public partial class AddCollectionPage : ComponentBase
{
    [Inject]
    public IWebManager<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel> MoviesCollectionsWebManager { get; set; }

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
    public IBrowserFile ImageToUpload { get; private set; }
    public string ImageSource { get; set; }

    const int MAX_FILESIZE = 5000 * 1024;

    public async Task AddMovieCollectionAsync()
    {
        var addGameCollectionModel = new AddMoviesCollectionModel(CollectionName, Description, ImageSource);

        HttpResponseMessage gameCreationHttpResponseMessage = await MoviesCollectionsWebManager.AddAsync(addGameCollectionModel);

        if (!gameCreationHttpResponseMessage.IsSuccessStatusCode)
            await JSRuntime.InvokeVoidAsync("alert", await gameCreationHttpResponseMessage.Content.ReadAsStringAsync());

        else
            NavigationManager.NavigateTo("/movies/collections");
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
