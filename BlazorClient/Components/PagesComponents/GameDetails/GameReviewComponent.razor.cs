

using Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Frontend;
using System.Threading.Tasks;
using WebManagers.Derived.Games;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameReviewComponent : ComponentBase
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

        if (authState is not null
            && authState.User is not null)
        {
            foreach (var claim in authState.User.Claims)
            {
                Console.WriteLine($"{claim.Type}\t{claim.Value}");
            }

            if (authState.User.Claims.FirstOrDefault(b => b.Type == ClaimTypes.Role
            && b.Value == "Admin") is not null
            || Convert.ToInt64(authState.User.Claims.FirstOrDefault(b => b.Type == ClaimTypes.NameIdentifier).Value) == AuthorId)
            {
                IsAbleToRemove = true;
            }
            if (Convert.ToInt64(authState.User.Claims.FirstOrDefault(b => b.Type == ClaimTypes.NameIdentifier).Value) == AuthorId)
            {
                IsAbleToEdit = true;
            }
        }
    }

    public async Task Like()
    {
        await GamesPlayersReviewsShiftsWebManager.AddAsync(new AddGamePlayerReviewShiftModel(Id, true));
        await OnUpdate.InvokeAsync(); // Вызываем обновление родителя
        StateHasChanged();
    }

    public async Task Dislike()
    {
        await GamesPlayersReviewsShiftsWebManager.AddAsync(new AddGamePlayerReviewShiftModel(Id, false));
        await OnUpdate.InvokeAsync(); // Вызываем обновление родителя
        StateHasChanged();
    }
}
