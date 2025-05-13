namespace Domain;
public sealed record DeveloperGame
{
    public long Id { get; set; }
    public long GameId { get; set; }
    public long DeveloperId { get; set; }
}
