using Domain.Games;
using Domain.Movies;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Platforms;
using Domain.RequestsModels.Movies.MoviesGenres;
using WebManagers;

namespace BlazorClient.Components.PagesComponents.Common;

public partial class Headerer : ComponentBase
{

    private bool isMenuOpen = false;
    private bool isLoginOpen = false;
    private bool isSticky = false;
    private string searchQuery = "";
    private ElementReference headerBottomRef;
    private IEnumerable<Domain.Movies.Genre>? moviesGenres;
    private IEnumerable<Platform>? platforms;

    // ===== ПАРАМЕТРЫ =====
    public IEnumerable<Domain.Movies.Genre>? MoviesGenres { get; set; }

    public IEnumerable<Platform>? Platforms { get; set; }

    public IEnumerable<Domain.Games.Genre>? GamesGenres { get; set; }

    // ===== INJECT =====
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider AuthProvider { get; set; } = default!;

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Inject]
    public IWebManager<Domain.Movies.Genre, AddMovieGenreModel, UpdateMovieGenreModel> MoviesGenresWebManager { get; set; }

    [Inject]
    public IWebManager<Platform, AddPlatformModel, UpdatePlatformModel> PlatformsWebManager { get; set; }

    [Inject]
    public IWebManager<Domain.Games.Genre, AddGameGenreModel, UpdateGameGenreModel> GamesGenresWebManager { get; set; }

    // ===== ЖИЗНЕННЫЙ ЦИКЛ =====
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Task<IEnumerable<Domain.Movies.Genre>> moviesGenresGettingTask = MoviesGenresWebManager.GetAllAsync();
            Task<IEnumerable<Platform>> platformsGettingTask = PlatformsWebManager.GetAllAsync();
            Task<IEnumerable<Domain.Games.Genre>> gamesGenresGettingTask = GamesGenresWebManager.GetAllAsync();

            await Task.WhenAll(moviesGenresGettingTask, platformsGettingTask, gamesGenresGettingTask)
                .ContinueWith(b =>
                {
                    MoviesGenres = moviesGenresGettingTask.Result;
                    Platforms = platformsGettingTask.Result;
                    GamesGenres = gamesGenresGettingTask.Result;
                    StateHasChanged();
                });

            // Инициализируем скролл и клики
            await JS.InvokeVoidAsync("initHeader", headerBottomRef);
        }
    }

    // ===== МЕТОДЫ =====
    private async Task ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        if (isMenuOpen)
        {
            await JS.InvokeVoidAsync("document.body.classList.add", "menu-open");
        }
        else
        {
            await JS.InvokeVoidAsync("document.body.classList.remove", "menu-open");
        }
    }

    private void Logout() => NavigationManager.NavigateTo("/auth/logout");

    private void OpenLogin()
    {
        isLoginOpen = true;
        // Закрываем меню если открыто
        if (isMenuOpen)
        {
            _ = ToggleMenu();
        }
    }

    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            Search();
        }
    }

    private async Task OnLoginSuccessHandler()
    {
        isLoginOpen = false;
        await AuthProvider.GetAuthenticationStateAsync();
        StateHasChanged();
    }

    private void Search()
    {
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            NavigationManager.NavigateTo($"/search?SearchText={Uri.EscapeDataString(searchQuery)}");
        }
    }
}