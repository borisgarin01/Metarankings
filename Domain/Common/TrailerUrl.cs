using System.Text.RegularExpressions;

namespace Domain.Common;

/// <summary>
/// Приводит ссылку на трейлер (YouTube / Rutube) к embed-формату для iframe.
/// Ссылки, которые не удалось распознать, возвращаются как есть.
/// </summary>
public static partial class TrailerUrl
{
    public static string? ToEmbed(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        url = url.Trim();

        Match match = YouTubeRegex().Match(url);
        if (match.Success)
            return $"https://www.youtube.com/embed/{match.Groups["id"].Value}";

        match = RutubeRegex().Match(url);
        if (match.Success)
            return $"https://rutube.ru/play/embed/{match.Groups["id"].Value}";

        return url;
    }

    // youtube.com/watch?v=ID, youtu.be/ID, youtube.com/embed/ID, youtube.com/shorts/ID
    [GeneratedRegex(@"^(?:https?://)?(?:www\.|m\.)?(?:youtube\.com/(?:watch\?(?:.*&)?v=|embed/|shorts/)|youtu\.be/)(?<id>[\w-]{11})", RegexOptions.IgnoreCase)]
    private static partial Regex YouTubeRegex();

    // rutube.ru/video/ID/, rutube.ru/play/embed/ID
    [GeneratedRegex(@"^(?:https?://)?(?:www\.)?rutube\.ru/(?:video|play/embed)/(?<id>[0-9a-f]{32})", RegexOptions.IgnoreCase)]
    private static partial Regex RutubeRegex();
}
