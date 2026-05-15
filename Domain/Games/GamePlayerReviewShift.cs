namespace Domain.Games;

public sealed record GamePlayerReviewShift(long Id, long GamePlayerReviewId, long ShifterId, bool Direction);
