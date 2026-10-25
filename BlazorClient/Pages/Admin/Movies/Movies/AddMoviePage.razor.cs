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

public sealed partial class AddMoviePage : ComponentBase
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

    public List<MovieDirector> SelectedMoviesDirectors { get; private set; } = new();
    public List<Genre> SelectedMoviesGenres { get; private set; } = new();
    public List<MovieStudio> SelectedMoviesStudios { get; private set; } = new();
    public List<MovieCountry> SelectedMoviesCountries { get; private set; } = new();

    private List<long> SelectedMoviesDirectorsIds => SelectedMoviesDirectors.Select(d => d.Id).ToList();
    private List<long> SelectedMoviesGenresIds => SelectedMoviesGenres.Select(g => g.Id).ToList();
    private List<long> SelectedMoviesStudiosIds => SelectedMoviesStudios.Select(s => s.Id).ToList();
    private List<long> SelectedMoviesCountriesIds => SelectedMoviesCountries.Select(c => c.Id).ToList();

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

    public Dictionary<MovieCrewRole, string> CrewTexts { get; } = MovieCrewEditor.CreateTexts();

    public IBrowserFile ImageToUpload { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        Task<IEnumerable<MovieDirector>> moviesDirectorsGetttingTask = MoviesDirectorsWebManager.GetAllAsync();
        Task<IEnumerable<Genre>> moviesGenresGettingTask = MoviesGenresWebManager.GetAllAsync();
        Task<IEnumerable<MovieStudio>> moviesStudiosGettingTask = MoviesStudiosWebManager.GetAllAsync();
        Task<IEnumerable<MovieCountry>> moviesCountriesGettingTask = MoviesCountriesWebManager.GetAllAsync();

        await Task.WhenAll(moviesDirectorsGetttingTask, moviesGenresGettingTask, moviesStudiosGettingTask, moviesCountriesGettingTask).ContinueWith(a =>
        {
            MoviesDirectorsToSelectFrom = moviesDirectorsGetttingTask.Result;
            MoviesGenresToSelectFrom = moviesGenresGettingTask.Result;
            MoviesStudiosToSelectFrom = moviesStudiosGettingTask.Result;
            MoviesCountriesToSelectFrom = moviesCountriesGettingTask.Result;
        });
    }

    private Task OnSelectedDirectorsChanged(IReadOnlyList<MovieDirector> list)
    {
        SelectedMoviesDirectors = list.ToList();
        return Task.CompletedTask;
    }

    private Task OnSelectedGenresChanged(IReadOnlyList<Genre> list)
    {
        SelectedMoviesGenres = list.ToList();
        return Task.CompletedTask;
    }

    private Task OnSelectedStudiosChanged(IReadOnlyList<MovieStudio> list)
    {
        SelectedMoviesStudios = list.ToList();
        return Task.CompletedTask;
    }

    private Task OnSelectedCountriesChanged(IReadOnlyList<MovieCountry> list)
    {
        SelectedMoviesCountries = list.ToList();
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
        return SelectedMoviesDirectorsIds.Count > 0
            && SelectedMoviesGenresIds.Count > 0
            && SelectedMoviesStudiosIds.Count > 0
            && ImageToUpload is not null
            && PremierDate.HasValue;
    }

    private async Task AddMovieAsync()
    {
        if (!MovieModelToAddConfigured())
        {
            ToastService.ShowWarning("Выберите режиссёра, жанр, студию, дату премьеры и загрузите постер.");
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

            string url = $"/api/movies/Images/{PremierDate!.Value.Year}/{PremierDate.Value.Month}/{uploadingFileNameWithCorrectExtention}";

            HttpResponseMessage response = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                string problemDetails = await response.Content.ReadAsStringAsync();
                ToastService.ShowError(problemDetails);
                return;
            }

            AddMovieModel addMovieModel = new AddMovieModel(
                Name: Name,
                OriginalName: OriginalName,
                Description: Description,
                ImageSource: url,
                PremierDate: PremierDate.Value,
                MoviesDirectorsNames: SelectedMoviesDirectors.Select(d => d.Name).ToList(),
                MoviesGenresNames: SelectedMoviesGenres.Select(g => g.Name).ToList(),
                MoviesStudiosNames: SelectedMoviesStudios.Select(s => s.Name).ToList(),
                Trailer: Trailer,
                MoviesCountriesNames: SelectedMoviesCountries.Select(c => c.Name).ToList(),
                MoviesCrew: MovieCrewEditor.ToModels(CrewTexts));

            HttpResponseMessage addingMovieResponseMessage = await MoviesWebManager.AddAsync(addMovieModel);

            if (addingMovieResponseMessage.IsSuccessStatusCode)
            {
                NavigationManager.NavigateTo("/admin/movies/list-movies");
            }
            else
            {
                string error = await addingMovieResponseMessage.Content.ReadAsStringAsync();
                ToastService.ShowError($"Failed to add movie: {error}");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError(ex.Message);
        }
    }
}