namespace Domain.Movies;

public sealed record MovieViewerReviewShift(long Id, long MovieViewerReviewId, long ShifterId, bool Direction);
