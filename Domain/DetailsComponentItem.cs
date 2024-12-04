namespace Domain;

public sealed record DetailsComponentItem
{
    public string Name { get; set; }
    public string ImageSource { get; set; }
    public float CriticsAverageScore { get; set; }
    public float GamersAverageScore { get; set; }
    public float Metarating { get; set; }
    public float ExpectationsPercent { get; set; }
    public int MarksCount { get; set; }
    public string OriginalName { get; set; }
    public Studio Studio { get; set; }
    public Genre[] MoviesGenres { get; set; }
    public Director Director { get; set; }
    public DateOnly PremiereDate { get; set; }
    public string Description { get; set; }
    public Actor[] Actors { get; set; }
    public Trailer[] Trailers { get; set; }
    public Screenshot[] Screenshots { get; set; }

}
