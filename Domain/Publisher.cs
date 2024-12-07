namespace Domain;

public sealed record Publisher
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Href { get; set; }
}