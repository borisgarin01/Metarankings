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

public sealed partial class EditMoviePage : CancellableComponentBase
{
    const int MAX_FILESIZE = 5000 * 1024;

    [Parameter]
    public long Id { get; set; }

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

    public Movie? Movie { get; private set; }
    public bool IsNotFound { get; private set; }
    private bool IsSaving { get; set; }

    public IEnumerable<MovieDirector> MoviesDirectorsToSelectFrom { get; private set; }
    public IEnumerable<Genre> MoviesGenresToSelectFrom { get; private set; }
    public IEnumerable<MovieStudio> MoviesStudiosToSelectFrom { get; private set; }
    public IEnumerable<MovieCountry> MoviesCountriesToSelectFrom { get; private set; }

    public string Name { get; set; }
    public string OriginalName { get; set; }
    public string Description { get; set; }
    public DateTime? PremierDate { get; set; }
    public string? Trailer { get; set; }

    public List<long> SelectedMoviesDirectorsIds { get; private set; } = new List<long>();
    public List<long> SelectedMoviesGenresIds { get; private set; } = new List<long>();
    public List<long> SelectedMoviesStudiosIds { get; private set; } = new List<long>();
    public List<long> SelectedMoviesCountriesIds { get; private set; } = new List<long>();

    public Dictionary<MovieCrewRole, string> CrewTexts { get; private set; } = MovieCrewEditor.CreateTexts();

    /// <summary>Data-url нового постера или текущий постер фильма.</summary>
    public string ImagePreview { get; private set; }
    public IBrowserFile? ImageToUpload { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        Task<IEnumerable<MovieDirector>> moviesDirectorsGettingTask = MoviesDirectorsWebManager.GetAllAsync(DisposalToken);
        Task<IEnumerable<Genre>> moviesGenresGettingTask = MoviesGenresWebManager.GetAllAsync(DisposalToken);
        Task<IEnumerable<MovieStudio>> moviesStudiosGettingTask = MoviesStudiosWebManager.GetAllAsync(DisposalToken);
        Task<IEnumerable<MovieCountry>> moviesCountriesGettingTask = MoviesCountriesWebManager.GetAllAsync(DisposalToken);

        try
        {
            Movie = await MoviesWebManager.GetAsync(Id, DisposalToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Movie = null;
        }

        await Task.WhenAll(moviesDirectorsGettingTask, moviesGenresGettingTask, moviesStudiosGettingTask, moviesCountriesGettingTask);

        MoviesDirectorsToSelectFrom = moviesDirectorsGettingTask.Result;
        MoviesGenresToSelectFrom = moviesGenresGettingTask.Result;
        MoviesStudiosToSelectFrom = moviesStudiosGettingTask.Result;
        MoviesCountriesToSelectFrom = moviesCountriesGettingTask.Result;

        if (Movie is null)
        {
            IsNotFound = true;
            return;
        }

        Name = Movie.Name;
        OriginalName = Movie.OriginalName;
        Description = Movie.Description;
        PremierDate = Movie.PremierDate?.ToDateTime(TimeOnly.MinValue);
        Trailer = Movie.Trailer;
        ImagePreview = Movie.ImageSource;

        SelectedMoviesDirectorsIds = Movie.MoviesDirectors.Select(d => d.Id).ToList();
        SelectedMoviesGenresIds = Movie.MovieGenres.Select(g => g.Id).ToList();
        SelectedMoviesStudiosIds = Movie.MoviesStudios.Select(s => s.Id).ToList();
        SelectedMoviesCountriesIds = Movie.MoviesCountries.Select(c => c.Id).ToList();
        CrewTexts = MovieCrewEditor.CreateTexts(Movie.MoviesCrew);
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
        if (string.IsNullOrWhiteSpace(OriginalName)) return "Укажите оригинальное название";
        if (!PremierDate.HasValue) return "Укажите дату премьеры";
        return null;
    }

    private async Task UpdateMovieAsync()
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
            string imageSource = Movie!.ImageSource;

            if (ImageToUpload is not null)
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                StreamContent fileContent = new StreamContent(ImageToUpload.OpenReadStream(50 * 1024 * 1024)); // 50MB max
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(ImageToUpload.ContentType);
                content.Add(fileContent, "formFile", ImageToUpload.Name);

                string uploadingImageName = Uri.EscapeDataString(Path.GetRandomFileName());
                string uploadingFileNameWithCorrectExtention = Path.ChangeExtension(uploadingImageName, Path.GetExtension(ImageToUpload.Name));
                string url = $"/api/movies/Images/{PremierDate!.Value.Year}/{PremierDate.Value.Month}/{uploadingFileNameWithCorrectExtention}";

                HttpResponseMessage response = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsync(url, content, DisposalToken);
                if (!response.IsSuccessStatusCode)
                {
                    ToastService.ShowError(await response.Content.ReadAsStringAsync(DisposalToken));
                    return;
                }

                imageSource = url;
            }

            UpdateMovieModel updateMovieModel = new UpdateMovieModel(
                Name: Name,
                OriginalName: OriginalName,
                Description: Description,
                ImageSource: imageSource,
                PremierDate: PremierDate!.Value,
                MoviesDirectorsIds: SelectedMoviesDirectorsIds,
                MoviesGenresIds: SelectedMoviesGenresIds,
                MoviesStudiosIds: SelectedMoviesStudiosIds,
                Trailer: Trailer,
                MoviesCountriesIds: SelectedMoviesCountriesIds,
                MoviesCrew: MovieCrewEditor.ToModels(CrewTexts));

            Movie? updatedMovie = await MoviesWebManager.UpdateAsync(Id, updateMovieModel, DisposalToken);

            if (updatedMovie is null)
            {
                ToastService.ShowError("Не удалось сохранить фильм");
                return;
            }

            ToastService.ShowSuccess("Фильм сохранён");
            NavigationManager.NavigateTo("/admin/movies/list-movies");
        }
        catch (Exception ex) when (!DisposalToken.IsCancellationRequested)
        {
            ToastService.ShowError(ex.Message);
        }
        finally
        {
            IsSaving = false;
        }
    }
}
