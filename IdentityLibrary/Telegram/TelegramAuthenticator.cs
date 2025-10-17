using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Settings;
using System;
using Telegram.Bot;

namespace IdentityLibrary.Telegram;

public sealed class TelegramAuthenticator
{
    public TelegramBotClient TelegramBotClient { get; }
    public ILogger<TelegramAuthenticator> Logger { get; }
    public IOptionsMonitor<AuthSettings> AuthSettings { get; }

    public TelegramAuthenticator(IOptionsMonitor<AuthSettings> authSettings, ILogger<TelegramAuthenticator> logger)
    {
        AuthSettings = authSettings;
        Logger = logger;
        TelegramBotClient = new TelegramBotClient(AuthSettings.CurrentValue.Telegram.Bot.Token);
    }

    public async Task SendMessageAsync(string message)
    {
        try
        {
            await TelegramBotClient.SendMessage(AuthSettings.CurrentValue.Telegram.Bot.ChatId, message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{ex.Message}\t{ex.StackTrace}");
        }
    }
}
