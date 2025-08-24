using IdentityLibrary.Telegram;

public sealed class NotificationsBackgroundService : BackgroundService
{
    private readonly TelegramAuthenticator _telegramAuthenticator;
    public NotificationsBackgroundService(TelegramAuthenticator telegramAuthenticator)
    {
        _telegramAuthenticator = telegramAuthenticator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _telegramAuthenticator.SendMessageAsync("Hello");
            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}