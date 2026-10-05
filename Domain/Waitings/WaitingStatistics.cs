namespace Domain.Waitings;

/// <summary>
/// Статистика ожидания игры/фильма: сколько пользователей ждут, сколько не ждут и голос текущего пользователя.
/// </summary>
public sealed record WaitingStatistics
{
    [JsonPropertyName("entityId")]
    public long EntityId { get; set; }

    [JsonPropertyName("waitingCount")]
    public int WaitingCount { get; set; }

    [JsonPropertyName("notWaitingCount")]
    public int NotWaitingCount { get; set; }

    /// <summary>
    /// true - жду, false - не жду, null - пользователь не голосовал (или не авторизован).
    /// </summary>
    [JsonPropertyName("userVote")]
    public bool? UserVote { get; set; }

    [JsonIgnore]
    public int VotesCount => WaitingCount + NotWaitingCount;

    [JsonIgnore]
    public int WaitingPercent => VotesCount == 0
        ? 0
        : (int)Math.Round(WaitingCount * 100.0 / VotesCount);
}
