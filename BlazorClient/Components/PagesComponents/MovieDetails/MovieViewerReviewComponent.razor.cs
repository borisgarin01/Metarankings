using Blazored.Toast.Services;
using Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Frontend;
using WebManagers.Derived.Movies;

namespace BlazorClient.Components.PagesComponents.MovieDetails;

public partial class MovieViewerReviewComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public long Id { get; set; }

    [Parameter, EditorRequired]
    public long AuthorId { get; set; }

    [Parameter, EditorRequired]
    public float Score { get; set; }

    [Parameter, EditorRequired]
    public string UserName { get; set; }

    [Parameter, EditorRequired]
    public DateTime PublishDate { get; set; }

    [Parameter, EditorRequired]
    public string TextContent { get; set; }

    [Parameter, EditorRequired]
    public int LikesCount { get; set; }

    [Parameter, EditorRequired]
    public int DislikesCount { get; set; }

    [Parameter]
    public EventCallback OnUpdate { get; set; }

    public bool IsAbleToRemove { get; private set; }

    public bool IsAbleToEdit { get; private set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public MoviesViewersReviewsShiftsWebManager MoviesViewersReviewsShiftsWebManager { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal? user = authState?.User;

        bool isAuthor = long.TryParse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out long userId)
            && userId == AuthorId;
        bool isAdmin = user?.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin") == true;

        IsAbleToEdit = isAuthor;
        IsAbleToRemove = isAuthor || isAdmin;
    }

    public Task Like() => ShiftAsync(true);

    public Task Dislike() => ShiftAsync(false);

    private async Task ShiftAsync(bool direction)
    {
        HttpResponseMessage response = await MoviesViewersReviewsShiftsWebManager.AddAsync(new AddMovieViewerReviewShiftModel(Id, direction));

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ToastService.ShowError("Войдите, чтобы оценивать отзывы");
            return;
        }

        if (!response.IsSuccessStatusCode)
        {
            ToastService.ShowError(await response.Content.ReadAsStringAsync());
            return;
        }

        await OnUpdate.InvokeAsync(); // Вызываем обновление родителя
        StateHasChanged();
    }
}
