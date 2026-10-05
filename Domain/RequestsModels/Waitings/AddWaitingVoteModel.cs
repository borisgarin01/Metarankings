namespace Domain.RequestsModels.Waitings;

/// <param name="EntityId">Id игры или фильма.</param>
/// <param name="IsWaiting">true - жду, false - не жду.</param>
public sealed record AddWaitingVoteModel(long EntityId, bool IsWaiting);
