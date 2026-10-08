namespace Domain.RequestsModels.Movies;

public sealed record MovieFilterRequest
{
    public long[]? GenresIds { get; set; }
    public long[]? MoviesStudiosIds { get; set; }
    public int[]? Years { get; set; }
    public long[]? MoviesDirectorsIds { get; set; }
    public long[]? MoviesCountriesIds { get; set; }
    public long[]? MoviesPersonsIds { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 10;
}