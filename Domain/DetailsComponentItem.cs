namespace Domain;

public sealed record DetailsComponentItem
{
    public string Name { get; set; }
    public string ImageSource { get; set; }
    public float AverageScore { get; set; }
    public int MarksCount { get; set; }
    public string OriginalName { get; set; }
    public Genre[] Genres { get; set; }
    public Director Director { get; set; }
    public DateOnly PremiereDate { get; set; }
    public string Description { get; set; }
    public Actor[] Actors { get; set; }
    public Trailer[] Trailers { get; set; }
    public Screenshot[] Screenshots { get; set; }
    
}
