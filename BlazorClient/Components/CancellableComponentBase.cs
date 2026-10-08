using System.Threading;

namespace BlazorClient;

/// <summary>
/// Базовый компонент с токеном, который отменяется при уничтожении компонента
/// (например, при уходе со страницы), чтобы прерывать незавершённые HTTP-запросы.
/// Отменённые задачи жизненного цикла и обработчиков событий Blazor игнорирует.
/// </summary>
public abstract class CancellableComponentBase : ComponentBase, IDisposable
{
    private readonly CancellationTokenSource _disposalTokenSource = new();

    /// <summary>
    /// Токен, отменяемый при уничтожении компонента.
    /// </summary>
    protected CancellationToken DisposalToken => _disposalTokenSource.Token;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Освобождение ресурсов наследника; переопределения должны вызывать базовую реализацию.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        // CancellationTokenSource без таймера не требует Dispose, а обращение
        // к Token после Dispose бросило бы ObjectDisposedException в поздних обработчиках.
        if (disposing)
            _disposalTokenSource.Cancel();
    }
}
