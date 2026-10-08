using Domain.Movies;

namespace BlazorClient.Pages.Movies.Movies;

public partial class MovieDetails : CancellableComponentBase
{
    private bool isAbleToWriteComments = false;

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }

    [Parameter]
    public long Id { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    public Movie Movie { get; private set; }

    private DateOnly PremierDate => Movie?.PremierDate ?? DateOnly.FromDateTime(DateTime.Now);

    public bool IsAbleToWriteComments
    {
        get => isAbleToWriteComments;
        private set
        {
            isAbleToWriteComments = value;
            StateHasChanged();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadMovieAsync();

        IsAbleToWriteComments = false;

        if (Movie is not null && AuthenticationState is not null)
        {
            AuthenticationState authState = await AuthenticationState;
            Claim? userIdClaim = authState?.User?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim is not null
                && long.TryParse(userIdClaim.Value, out long userId)
                && Movie.MovieReviews.FirstOrDefault(mr => mr.ViewerId == userId) is null)
            {
                IsAbleToWriteComments = true;
            }
        }
        StateHasChanged();
    }

    private async Task LoadMovieAsync()
    {
        Movie = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Movie>($"/api/movies/{Id}", DisposalToken);
    }

    // Вызывается из MovieViewerReviewComponent после лайка/дизлайка
    private async Task RefreshMovieData()
    {
        await LoadMovieAsync();
        StateHasChanged();
    }
}
