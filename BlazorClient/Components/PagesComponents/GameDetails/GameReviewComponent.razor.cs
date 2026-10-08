using Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Frontend;
using WebManagers.Derived.Games;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameReviewComponent : CancellableComponentBase
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
    public DateOnly PublishDate { get; set; }

    [Parameter, EditorRequired]
    public string TextContent { get; set; }

    [Parameter, EditorRequired]
    public int LikesCount { get; set; }

    [Parameter, EditorRequired]
    public int DislikesCount { get; set; }

    public bool IsAbleToRemove { get; private set; }

    public bool IsAbleToEdit { get; private set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public GamesPlayersReviewsShiftsWebManager GamesPlayersReviewsShiftsWebManager { get; set; }
    [Parameter]
    public EventCallback OnUpdate { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal? user = authState?.User;

        // У анонимного пользователя нет NameIdentifier — он не автор и не администратор
        bool isAuthor = long.TryParse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out long userId)
            && userId == AuthorId;
        bool isAdmin = user?.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin") == true;

        IsAbleToEdit = isAuthor;
        IsAbleToRemove = isAuthor || isAdmin;
    }

    public async Task Like()
    {
        await GamesPlayersReviewsShiftsWebManager.AddAsync(new AddGamePlayerReviewShiftModel(Id, true), DisposalToken);
        await OnUpdate.InvokeAsync(); // Вызываем обновление родителя
        StateHasChanged();
    }

    public async Task Dislike()
    {
        await GamesPlayersReviewsShiftsWebManager.AddAsync(new AddGamePlayerReviewShiftModel(Id, false), DisposalToken);
        await OnUpdate.InvokeAsync(); // Вызываем обновление родителя
        StateHasChanged();
    }
}
