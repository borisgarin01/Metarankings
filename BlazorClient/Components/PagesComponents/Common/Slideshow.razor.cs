using BlazorClient.Models;
using System.Threading;

namespace BlazorClient.Components.PagesComponents.Common;

/// <summary>
/// Слайдер в стиле оригинального сайта: эффект scrollHorz из jQuery Cycle2
/// (скорость 500 мс, пауза между слайдами — <see cref="Interval"/>).
/// </summary>
public partial class Slideshow : ComponentBase, IAsyncDisposable
{
    /// <summary>Длительность прокрутки слайда (значение speed по умолчанию в Cycle2).</summary>
    private const int Speed = 500;

    /// <summary>Пауза, за которую браузер успевает применить стартовые позиции перед анимацией.</summary>
    private const int FrameDelay = 50;

    /// <summary>Аналог easing "swing" из jQuery, которым анимирует Cycle2.</summary>
    private const string Easing = "cubic-bezier(0.445, 0.05, 0.55, 0.95)";

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

    private enum Phase { Idle, Prepare, Animate }

    private List<SlideGroup> _groups = new();
    private int CurrentIndex { get; set; }

    private int _prevIndex = -1;
    private int _direction = 1;
    private Phase _phase = Phase.Idle;
    private bool _busy;
    private bool _progressRunning;

    private CancellationTokenSource? _cts;

    protected override void OnParametersSet()
    {
        _groups = Groups?.ToList() ?? new List<SlideGroup>();
        if (_groups.Count == 0 || CurrentIndex >= _groups.Count)
        {
            CurrentIndex = 0;
            _prevIndex = -1;
            _phase = Phase.Idle;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        // cycle-initialized: полоса прогресса стартует с нуля и заполняется за Interval
        await Task.Delay(FrameDelay);
        StartProgress();
        RestartTimer();
    }

    private string SlideStyle(int index)
    {
        const string position = "position: absolute; top: 0px; ";

        if (index == CurrentIndex)
        {
            return _phase switch
            {
                Phase.Prepare => position + $"left: {_direction * 100}%; transition: none;",
                Phase.Animate => position + $"left: 0%; transition: left {Speed}ms {Easing};",
                _ => position + "left: 0%;"
            };
        }

        if (index == _prevIndex && _phase != Phase.Idle)
        {
            return _phase == Phase.Prepare
                ? position + "left: 0%; transition: none;"
                : position + $"left: {-_direction * 100}%; transition: left {Speed}ms {Easing};";
        }

        return position + "left: 0%; visibility: hidden;";
    }

    private string ProgressStyle => _progressRunning
        ? $"width: 100%; transition: width {Interval}ms linear;"
        : "width: 0;";

    private void StartProgress()
    {
        if (!Autoplay || _groups.Count < 2) return;
        _progressRunning = true;
        StateHasChanged();
    }

    private async Task TransitionTo(int index, int direction)
    {
        if (_busy || index == CurrentIndex || index < 0 || index >= _groups.Count) return;
        _busy = true;

        // cycle-before: входящий слайд встаёт рядом с текущим, прогресс сбрасывается
        _prevIndex = CurrentIndex;
        CurrentIndex = index;
        _direction = direction;
        _phase = Phase.Prepare;
        _progressRunning = false;
        StateHasChanged();
        await OnSlideChanged.InvokeAsync(CurrentIndex);

        await Task.Delay(FrameDelay);
        _phase = Phase.Animate;
        StateHasChanged();

        await Task.Delay(Speed);

        // cycle-after
        _phase = Phase.Idle;
        _prevIndex = -1;
        _busy = false;
        StateHasChanged();
        await Task.Delay(FrameDelay);
        StartProgress();
    }

    private async Task RunTimerAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(Interval + FrameDelay, token);
                if (_groups.Count < 2) continue;
                await InvokeAsync(() => TransitionTo((CurrentIndex + 1) % _groups.Count, 1));
                await Task.Delay(Speed + 2 * FrameDelay, token);
            }
        }
        catch (OperationCanceledException)
        {
            // нормальное завершение при Dispose или ручном переключении
        }
    }

    private async Task GoTo(int index)
    {
        if (_busy || index < 0 || index >= _groups.Count || index == CurrentIndex) return;

        _cts?.Cancel();
        await TransitionTo(index, index > CurrentIndex ? 1 : -1);
        RestartTimer();
    }

    private void RestartTimer()
    {
        if (!Autoplay) return;
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        _ = RunTimerAsync(_cts.Token);
    }

    public ValueTask DisposeAsync()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        return ValueTask.CompletedTask;
    }

    private static string GetMarkClass(float? score)
        => score is null ? "0" : ((int)Math.Round(score.Value)).ToString();
}
