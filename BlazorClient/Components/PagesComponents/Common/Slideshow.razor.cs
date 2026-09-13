using BlazorClient.Models;
using System.Threading;

namespace BlazorClient.Components.PagesComponents.Common;

public partial class Slideshow : ComponentBase, IAsyncDisposable
{
    [Parameter, EditorRequired]
    public IEnumerable<SlideGroup> Groups { get; set; } = Enumerable.Empty<SlideGroup>();

    [Parameter] 
    public int Interval { get; set; } = 7000;
    
    [Parameter] 
    public bool Autoplay { get; set; } = true;
    
    [Parameter] 
    public bool ShowPager { get; set; } = true;
    
    [Parameter] 
    public bool ShowArrows { get; set; } = true;

    /// <summary>Внешний обработчик смены слайда (для связки с внешним UI).</summary>
    [Parameter] public EventCallback<int> OnSlideChanged { get; set; }

    private List<SlideGroup> _groups = new();
    private int CurrentIndex { get; set; }

    private PeriodicTimer? _timer;
    private CancellationTokenSource? _cts;

    protected override void OnParametersSet()
    {
        _groups = Groups?.ToList() ?? new List<SlideGroup>();
        if (_groups.Count == 0) CurrentIndex = 0;
        else if (CurrentIndex >= _groups.Count) CurrentIndex = 0;
    }

    protected override void OnInitialized()
    {
        if (!Autoplay) return;
        _cts = new CancellationTokenSource();
        _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(Interval));
        _ = RunTimerAsync(_cts.Token);
    }

    private async Task RunTimerAsync(CancellationToken token)
    {
        try
        {
            while (await _timer!.WaitForNextTickAsync(token))
            {
                if (_groups.Count < 2) continue;
                await InvokeAsync(() =>
                {
                    CurrentIndex = (CurrentIndex + 1) % _groups.Count;
                    StateHasChanged();
                    return OnSlideChanged.InvokeAsync(CurrentIndex);
                });
            }
        }
        catch (OperationCanceledException)
        {
            // нормальное завершение при Dispose
        }
    }

    private async Task GoTo(int index)
    {
        if (index < 0 || index >= _groups.Count) return;
        if (index == CurrentIndex) return;

        RestartTimer();

        CurrentIndex = index;
        StateHasChanged();
        await OnSlideChanged.InvokeAsync(CurrentIndex);
    }

    private void RestartTimer()
    {
        if (!Autoplay) return;
        _cts?.Cancel();
        _timer?.Dispose();
        _cts = new CancellationTokenSource();
        _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(Interval));
        _ = RunTimerAsync(_cts.Token);
    }

    public ValueTask DisposeAsync()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _timer?.Dispose();
        return ValueTask.CompletedTask;
    }

    private static string GetMarkClass(float? score)
    => score is null ? "0" : ((int)Math.Round(score.Value)).ToString();
}