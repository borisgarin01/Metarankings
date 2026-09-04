using System.Timers;

namespace BlazorClient.Pages;

public partial class Carousel : ComponentBase, IDisposable, IAsyncDisposable
{
    [Parameter] public int Interval { get; set; } = 5000;
    [Parameter] public bool Autoplay { get; set; } = true;
    [Parameter] public bool ShowControls { get; set; } = true;
    [Parameter] public bool ShowIndicators { get; set; } = true;
    [Parameter] public bool ShowProgress { get; set; } = true;
    [Parameter] public bool ShowSlideInfo { get; set; } = false;
    [Parameter] public string ProgressPosition { get; set; } = "bottom";
    [Parameter] public string AdditionalClass { get; set; } = "";
    [Parameter] public string ContainerClass { get; set; } = "";
    [Parameter] public EventCallback<int> OnSlideChanged { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    // ========== Приватные поля ==========
    private List<CarouselItem> _items = new();
    private int CurrentIndex { get; set; } = 0;
    private double ProgressWidth { get; set; } = 0;
    private bool IsPaused { get; set; } = false;
    private System.Timers.Timer? _timer;
    private DateTime _lastUpdate;
    private string CarouselId = "carousel-" + Guid.NewGuid().ToString()[..8];
    private bool _isDisposed;
    private bool _isInitialized;

    // ========== Методы управления ==========
    public void AddItem(CarouselItem item)
    {
        if (!_items.Contains(item))
        {
            item.Index = _items.Count;
            _items.Add(item);

            if (_items.Count == 1)
            {
                item.IsActive = true;
                CurrentIndex = 0;
            }

            StateHasChanged();
        }
    }

    public async Task GoToNext()
    {
        if (_items.Count == 0) return;

        ResetProgress();
        _items[CurrentIndex].IsActive = false;
        CurrentIndex = (CurrentIndex + 1) % _items.Count;
        _items[CurrentIndex].IsActive = true;

        await OnSlideChanged.InvokeAsync(CurrentIndex);
        StateHasChanged();
    }

    public async Task GoToPrevious()
    {
        if (_items.Count == 0) return;

        ResetProgress();
        _items[CurrentIndex].IsActive = false;
        CurrentIndex = (CurrentIndex - 1 + _items.Count) % _items.Count;
        _items[CurrentIndex].IsActive = true;

        await OnSlideChanged.InvokeAsync(CurrentIndex);
        StateHasChanged();
    }

    public async Task GoToSlide(int index)
    {
        if (index < 0 || index >= _items.Count || index == CurrentIndex) return;

        ResetProgress();
        _items[CurrentIndex].IsActive = false;
        CurrentIndex = index;
        _items[CurrentIndex].IsActive = true;

        await OnSlideChanged.InvokeAsync(CurrentIndex);
        StateHasChanged();
    }

    public void PauseAutoplay()
    {
        if (!Autoplay) return;
        IsPaused = true;
    }

    public void ResumeAutoplay()
    {
        if (!Autoplay) return;
        IsPaused = false;
        _lastUpdate = DateTime.Now;
    }

    private void ResetProgress()
    {
        ProgressWidth = 0;
        _lastUpdate = DateTime.Now;
    }

    // ========== Логика прогресса ==========
    private void StartAutoplay()
    {
        if (!Autoplay || _timer != null || _items.Count <= 1) return;

        _timer = new System.Timers.Timer(16);
        _timer.Elapsed += UpdateProgress;
        _timer.AutoReset = true;
        _timer.Start();
        _lastUpdate = DateTime.Now;
        _isInitialized = true;
    }

    private void UpdateProgress(object? sender, ElapsedEventArgs e)
    {
        if (IsPaused || _items.Count <= 1) return;

        var elapsed = (DateTime.Now - _lastUpdate).TotalMilliseconds;
        var progressIncrement = (elapsed / Interval) * 100;

        InvokeAsync(() =>
        {
            ProgressWidth = Math.Min(100, ProgressWidth + progressIncrement);
            StateHasChanged();

            if (ProgressWidth >= 100)
            {
                _ = GoToNext();
            }
        });

        _lastUpdate = DateTime.Now;
    }

    // ========== Жизненный цикл ==========
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            StartAutoplay();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_isInitialized && Autoplay && _timer == null && _items.Count > 1)
        {
            StartAutoplay();
        }
        else if (_isInitialized && !Autoplay && _timer != null)
        {
            await StopAutoplayAsync();
        }
    }

    private async Task StopAutoplayAsync()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Elapsed -= UpdateProgress;
            _timer.Dispose();
            _timer = null;
        }
        await Task.CompletedTask;
    }

    // ========== Реализация IDisposable ==========
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Elapsed -= UpdateProgress;
                _timer.Dispose();
                _timer = null;
            }
        }

        _isDisposed = true;
    }

    // ========== Реализация IAsyncDisposable ==========
    public async ValueTask DisposeAsync()
    {
        // Выполняем асинхронную очистку
        await DisposeAsyncCore();

        // Затем синхронную
        Dispose(false);

        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        // У System.Timers.Timer нет DisposeAsync, используем синхронный Dispose
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Elapsed -= UpdateProgress;
            _timer.Dispose();
            _timer = null;
        }

        await Task.CompletedTask; // Для соблюдения async-сигнатуры
    }
}