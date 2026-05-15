namespace Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend;

public sealed record UpdateGamePlayerReviewShiftModel(long GamePlayerReviewId, long ShifterId, bool Direction);
