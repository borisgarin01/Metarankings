using Blazored.Toast.Services;
using Domain.Waitings;
using System.Net;
using WebManagers.Derived.Waitings;

namespace BlazorClient.Components.PagesComponents.Common;

/// <summary>
/// Кнопки "жду / не жду" для игры или фильма.
/// Счетчики можно передать снаружи (списки) или загрузить самостоятельно (LoadStatistics = true).
/// </summary>
public partial class WaitingVoteComponent : ComponentBase
{
    private int waitingCount;
    private int notWaitingCount;
    private bool? userVote;
    private bool isSending;

    // Последние полученные значения параметров: локальное состояние перезаписывается только при их изменении,
    // иначе повторный рендер родителя затер бы результат голосования.
    private (long EntityId, int WaitingCount, int NotWaitingCount, bool? UserVote)? lastParameters;

    [Parameter, EditorRequired]
    public long EntityId { get; set; }

    [Parameter, EditorRequired]
    public WaitingTarget Target { get; set; }

    [Parameter]
    public int WaitingCount { get; set; }

    [Parameter]
    public int NotWaitingCount { get; set; }

    [Parameter]
    public bool? UserVote { get; set; }

    [Parameter]
    public bool LoadStatistics { get; set; }

    [Parameter]
    public bool ShowEmptyHint { get; set; }

    [Inject]
    public GamesWaitingsWebManager GamesWaitingsWebManager { get; set; } = default!;

    [Inject]
    public MoviesWaitingsWebManager MoviesWaitingsWebManager { get; set; } = default!;

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    private WaitingsWebManager WebManager => Target == WaitingTarget.Game
        ? GamesWaitingsWebManager
        : MoviesWaitingsWebManager;

    private int WaitingsCount => waitingCount + notWaitingCount;

    private int WaitingPercent => WaitingsCount == 0
        ? 0
        : (int)Math.Round(waitingCount * 100.0 / WaitingsCount);

    protected override async Task OnParametersSetAsync()
    {
        var parameters = (EntityId, WaitingCount, NotWaitingCount, UserVote);
        if (lastParameters == parameters)
            return;

        bool entityChanged = lastParameters?.EntityId != EntityId;
        lastParameters = parameters;

        waitingCount = WaitingCount;
        notWaitingCount = NotWaitingCount;
        userVote = UserVote;

        if (LoadStatistics && entityChanged)
        {
            try
            {
                Apply(await WebManager.GetAsync(EntityId));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load waiting statistics: {ex.Message}");
            }
        }
    }

    private async Task VoteAsync(bool isWaiting)
    {
        AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated != true)
        {
            ToastService.ShowInfo("Войдите на сайт, чтобы отметить ожидание");
            return;
        }

        isSending = true;
        try
        {
            HttpResponseMessage response = await WebManager.VoteAsync(EntityId, isWaiting);

            if (response.IsSuccessStatusCode)
                Apply(await response.Content.ReadFromJsonAsync<WaitingStatistics>());
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
                ToastService.ShowInfo("Войдите на сайт, чтобы отметить ожидание");
            else
                ToastService.ShowError("Не удалось сохранить голос");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to vote: {ex.Message}");
            ToastService.ShowError("Не удалось сохранить голос");
        }
        finally
        {
            isSending = false;
        }
    }

    private void Apply(WaitingStatistics? statistics)
    {
        if (statistics is null)
            return;

        waitingCount = statistics.WaitingCount;
        notWaitingCount = statistics.NotWaitingCount;
        userVote = statistics.UserVote;
    }
}

public enum WaitingTarget
{
    Game,
    Movie
}
