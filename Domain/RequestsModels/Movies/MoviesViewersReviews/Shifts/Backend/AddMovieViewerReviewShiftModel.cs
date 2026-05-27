namespace Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend;

public sealed record AddMovieViewerReviewShiftModel(long MovieViewerReviewId, long ShifterId, bool Direction);