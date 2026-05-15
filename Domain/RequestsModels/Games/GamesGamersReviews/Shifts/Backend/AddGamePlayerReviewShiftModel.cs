namespace Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend;

public sealed record AddGamePlayerReviewShiftModel(long GamePlayerReviewId, long ShifterId, bool Direction);