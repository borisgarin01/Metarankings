using Microsoft.Extensions.Logging;
using System;
using Telegram.Bot;

namespace IdentityLibrary.Telegram;

public sealed class TelegramAuthenticator
{
    public TelegramBotClient TelegramBotClient { get; }
    public IConfiguration Configuration { get; }
    public ILogger<TelegramAuthenticator> Logger { get; }

    public TelegramAuthenticator(IConfiguration configuration, ILogger<TelegramAuthenticator> logger)
    {
        Configuration = configuration;
        Logger = logger;
        TelegramBotClient = new TelegramBotClient(Configuration["Auth:Telegram:Bot:Token"]);
    }

    public async Task SendMessageAsync(string message)
    {
        try
        {
            await TelegramBotClient.SendMessage(Configuration["Auth:Telegram:Bot:ChatId"], message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{ex.Message}\t{ex.StackTrace}");
        }
    }
}
