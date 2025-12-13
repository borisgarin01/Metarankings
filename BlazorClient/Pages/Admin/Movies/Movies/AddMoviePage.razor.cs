using Domain.Movies;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Movies.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using Domain.RequestsModels.Movies.MoviesGenres;
using Domain.RequestsModels.Movies.MoviesStudios;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.Movies;

public sealed partial class AddMoviePage : ComponentBase
{
    const int MAX_FILESIZE = 5000 * 1024;

    protected override async Task OnInitializedAsync()
    {
        Task<IEnumerable<MovieDirector>> moviesDirectorsGetttingTask = MoviesDirectorsWebManager.GetAllAsync();
        Task<IEnumerable<MovieGenre>> moviesGenresGettingTask = MoviesGenresWebManager.GetAllAsync();
        Task<IEnumerable<MovieStudio>> moviesStudiosGettingTask = MoviesStudiosWebManager.GetAllAsync();

        await Task.WhenAll(moviesDirectorsGetttingTask, moviesGenresGettingTask, moviesStudiosGettingTask).ContinueWith(a =>
        {
            MoviesDirectorsToSelectFrom = moviesDirectorsGetttingTask.Result;
            MoviesGenresToSelectFrom = moviesGenresGettingTask.Result;
            MoviesStudiosToSelectFrom = moviesStudiosGettingTask.Result;
        });
    }

    [Inject]
    public HttpClient HttpClient { get; set; }

    [Inject]
    public IWebManager<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel> MoviesDirectorsWebManager { get; set; }

    [Inject]
    public IWebManager<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel> MoviesGenresWebManager { get; set; }

    [Inject]
    public IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel> MoviesStudiosWebManager { get; set; }

    [Inject]
    public IWebManager<Movie, AddMovieModel, UpdateMovieModel> MoviesWebManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public IEnumerable<MovieDirector> MoviesDirectorsToSelectFrom { get; set; }
    public IEnumerable<MovieGenre> MoviesGenresToSelectFrom { get; set; }
    public IEnumerable<MovieStudio> MoviesStudiosToSelectFrom { get; set; }

    public List<MovieDirector> SelectedMoviesDirectors { get; set; } = new List<MovieDirector>();
    public List<MovieGenre> SelectedGenres { get; set; } = new List<MovieGenre>();
    public List<MovieStudio> SelectedMoviesStudios { get; set; } = new List<MovieStudio>();

    [EditorRequired]
    public string Name { get; set; }

    [EditorRequired]
    public string OriginalName { get; set; }

    [EditorRequired]
    public string Description { get; set; }
    public string ImageSource { get; private set; }

    [EditorRequired]
    public DateTime? PremierDate { get; set; }

    public IEnumerable<long> SelectedMoviesDirectorsIds { get; private set; }
    public IEnumerable<long> SelectedMoviesGenresIds { get; private set; }
    public IEnumerable<long> SelectedMoviesStudiosIds { get; private set; }

    public IBrowserFile ImageToUpload { get; private set; }

    private Task SelectMovieDirector(ChangeEventArgs e)
    {
        SelectedMoviesDirectorsIds = ((string[])e.Value)
            .Select(idString => long.Parse(idString))
            .ToList();

        return Task.CompletedTask;
    }

    private Task SelectMovieGenre(ChangeEventArgs e)
    {
        SelectedMoviesGenresIds = ((string[])e.Value)
            .Select(idString => long.Parse(idString))
            .ToList();

        return Task.CompletedTask;
    }

    private Task SelectMovieStudio(ChangeEventArgs e)
    {
        SelectedMoviesStudiosIds = ((string[])e.Value)
            .Select(idString => long.Parse(idString))
            .ToList();

        return Task.CompletedTask;
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

    private bool MovieModelToAddConfigured()
    {
        return !SelectedMoviesDirectorsIds.Contains(-1)
            && !SelectedMoviesGenresIds.Contains(-1)
            && !SelectedMoviesStudiosIds.Contains(-1)
            && ImageToUpload is not null;
    }

    private async Task AddMovieAsync()
    {
        if (MovieModelToAddConfigured())
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

                var url = $"api/movies/Images/{PremierDate.Value.Year}/{PremierDate.Value.Month}/{uploadingFileNameWithCorrectExtention}";

                // Send the request with authentication token
                var response = await HttpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var addMovieModel = new AddMovieModel(
                        Name: Name,
                        OriginalName: OriginalName,
                        Description: Description,
                        ImageSource: uploadingFileNameWithCorrectExtention,
                        PremierDate: PremierDate.Value,
                        MoviesDirectorsNames: MoviesDirectorsToSelectFrom
                            .Where(d => SelectedMoviesDirectorsIds.Contains(d.Id)).Select(b => b.Name)
                            .ToList(),
                        MoviesGenresNames: MoviesGenresToSelectFrom
                            .Where(g => SelectedMoviesGenresIds.Contains(g.Id))
                            .Select(b => b.Name)
                            .ToList(),
                        MoviesStudiosNames: MoviesStudiosToSelectFrom
                            .Where(s => SelectedMoviesStudiosIds.Contains(s.Id))
                            .Select(b => b.Name)
                            .ToList()
                    );

                    HttpResponseMessage addingMovieResponseMessage = await MoviesWebManager.AddAsync(addMovieModel);

                    if (addingMovieResponseMessage.IsSuccessStatusCode)
                    {
                        NavigationManager.NavigateTo("/movies/movies/movies-list");
                    }
                    else
                    {
                        var error = await addingMovieResponseMessage.Content.ReadAsStringAsync();
                        await JSRuntime.InvokeVoidAsync("alert", $"Failed to add movie: {error}");
                    }
                }
                else
                {
                    var problemDetails = await response.Content.ReadAsStringAsync();
                    await JSRuntime.InvokeVoidAsync("alert", problemDetails);
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }
    }
}