
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

    public bool IsAbleToRemove { get; private set; }

    public bool IsAbleToEdit { get; private set; }

    [Inject]
    public AuthenticationStateProvider GetAuthenticationStateAsync { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AuthenticationState authState = await GetAuthenticationStateAsync.GetAuthenticationStateAsync();

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
}
