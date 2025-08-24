using Telegram.Bot;

namespace IdentityLibrary.Telegram;

public sealed class TelegramAuthenticator
{
    public string Token { get; }
    public string ChatId { get; }
    public TelegramBotClient TelegramBotClient { get; }

    public TelegramAuthenticator(string token, string chatId)
    {
        Token = token;
        ChatId = chatId;
        TelegramBotClient = new TelegramBotClient(token);
    }

    public async Task SendMessageAsync(string message)
    {
        await TelegramBotClient.SendMessage(ChatId, message);
    }
}
