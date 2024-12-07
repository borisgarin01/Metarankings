namespace Domain;
public sealed record Developer
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Href { get; set; }
}
