namespace Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend;

public sealed record UpdateMovieViewerReviewShiftModel(long MovieViewerReviewId, long ShifterId, bool Direction);
