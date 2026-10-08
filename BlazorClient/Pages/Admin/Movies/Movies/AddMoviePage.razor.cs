using Blazored.Toast.Services;
using Domain.Movies;
using Domain.RequestsModels.Movies.Movies;
using Domain.RequestsModels.Movies.MoviesCountries;
using Domain.RequestsModels.Movies.MoviesDirectors;
using Domain.RequestsModels.Movies.MoviesGenres;
using Domain.RequestsModels.Movies.MoviesStudios;
using BlazorClient.Components.PagesComponents.MovieDetails;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using WebManagers;
using WebManagers.Derived.Movies;

namespace BlazorClient.Pages.Admin.Movies.Movies;

public sealed partial class AddMoviePage : CancellableComponentBase
{
    const int MAX_FILESIZE = 5000 * 1024;

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    [Inject]
    public IWebManager<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel> MoviesDirectorsWebManager { get; set; }

    [Inject]
    public IWebManager<Genre, AddMovieGenreModel, UpdateMovieGenreModel> MoviesGenresWebManager { get; set; }

    [Inject]
    public IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel> MoviesStudiosWebManager { get; set; }

    [Inject]
    public IWebManager<MovieCountry, AddMovieCountryModel, UpdateMovieCountryModel> MoviesCountriesWebManager { get; set; }

    [Inject]
    public MoviesWebManager MoviesWebManager { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public IEnumerable<MovieDirector> MoviesDirectorsToSelectFrom { get; set; }
    public IEnumerable<Genre> MoviesGenresToSelectFrom { get; set; }
    public IEnumerable<MovieStudio> MoviesStudiosToSelectFrom { get; set; }
    public IEnumerable<MovieCountry> MoviesCountriesToSelectFrom { get; set; }

    public List<MovieDirector> SelectedMoviesDirectors { get; set; } = new List<MovieDirector>();
    public List<Genre> SelectedMoviesGenres { get; set; } = new List<Genre>();
    public List<MovieStudio> SelectedMoviesStudios { get; set; } = new List<MovieStudio>();

    [EditorRequired]
    public string Name { get; set; }

    [EditorRequired]
    public string OriginalName { get; set; }

    [EditorRequired]
    public string Description { get; set; }
    public string ImageSource { get; private set; }

    public string? Trailer { get; set; }

    [EditorRequired]
    public DateTime? PremierDate { get; set; }

    public List<long> SelectedMoviesDirectorsIds { get; private set; } = new List<long>();
    public List<long> SelectedMoviesGenresIds { get; private set; } = new List<long>();
    public List<long> SelectedMoviesStudiosIds { get; private set; } = new List<long>();
    public List<long> SelectedMoviesCountriesIds { get; private set; } = new List<long>();

    public Dictionary<MovieCrewRole, string> CrewTexts { get; } = MovieCrewEditor.CreateTexts();

    public IBrowserFile ImageToUpload { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        Task<IEnumerable<MovieDirector>> moviesDirectorsGetttingTask = MoviesDirectorsWebManager.GetAllAsync(DisposalToken);
        Task<IEnumerable<Genre>> moviesGenresGettingTask = MoviesGenresWebManager.GetAllAsync(DisposalToken);
        Task<IEnumerable<MovieStudio>> moviesStudiosGettingTask = MoviesStudiosWebManager.GetAllAsync(DisposalToken);
        Task<IEnumerable<MovieCountry>> moviesCountriesGettingTask = MoviesCountriesWebManager.GetAllAsync(DisposalToken);

        await Task.WhenAll(moviesDirectorsGetttingTask, moviesGenresGettingTask, moviesStudiosGettingTask, moviesCountriesGettingTask);
        MoviesDirectorsToSelectFrom = moviesDirectorsGetttingTask.Result;
        MoviesGenresToSelectFrom = moviesGenresGettingTask.Result;
        MoviesStudiosToSelectFrom = moviesStudiosGettingTask.Result;
        MoviesCountriesToSelectFrom = moviesCountriesGettingTask.Result;
    }

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
        using Stream imageToUploadReadStream = ImageToUpload.OpenReadStream(MAX_FILESIZE);
        using MemoryStream memoryStream = new MemoryStream();
        await imageToUploadReadStream.CopyToAsync(memoryStream);
        ImageSource = $"data:{ImageToUpload.ContentType};base64,{Convert.ToBase64String(memoryStream.ToArray())}";
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
                MultipartFormDataContent content = new MultipartFormDataContent();
                StreamContent fileContent = new StreamContent(ImageToUpload.OpenReadStream(50 * 1024 * 1024)); // 50MB max
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(ImageToUpload.ContentType);
                content.Add(fileContent, "formFile", ImageToUpload.Name);

                string uploadingImageName = Uri.EscapeDataString(Path.GetRandomFileName());
                string uploadingFileNameWithCorrectExtention = Path.ChangeExtension(uploadingImageName, Path.GetExtension(ImageToUpload.Name));

                string url = $"/api/movies/Images/{PremierDate.Value.Year}/{PremierDate.Value.Month}/{uploadingFileNameWithCorrectExtention}";

                // Send the request with authentication token
                var response = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsync(url, content, DisposalToken);

                if (response.IsSuccessStatusCode)
                {
                    AddMovieModel addMovieModel = new AddMovieModel(
                        Name: Name,
                        OriginalName: OriginalName,
                        Description: Description,
                        ImageSource: url,
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
                            .ToList(),
                        Trailer: Trailer,
                        MoviesCountriesNames: MoviesCountriesToSelectFrom
                            .Where(c => SelectedMoviesCountriesIds.Contains(c.Id))
                            .Select(c => c.Name)
                            .ToList(),
                        MoviesCrew: MovieCrewEditor.ToModels(CrewTexts)
                    );

                    HttpResponseMessage addingMovieResponseMessage = await MoviesWebManager.AddAsync(addMovieModel, DisposalToken);

                    if (addingMovieResponseMessage.IsSuccessStatusCode)
                    {
                        NavigationManager.NavigateTo("/admin/movies/list-movies");
                    }
                    else
                    {
                        string error = await addingMovieResponseMessage.Content.ReadAsStringAsync(DisposalToken);
                        ToastService.ShowError($"Failed to add movie: {error}");
                    }
                }
                else
                {
                    string problemDetails = await response.Content.ReadAsStringAsync(DisposalToken);
                    ToastService.ShowError(problemDetails);
                }
            }
            catch (Exception ex) when (!DisposalToken.IsCancellationRequested)
            {
                ToastService.ShowError(ex.Message);
            }
        }
    }
}