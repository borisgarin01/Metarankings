using System.Text.Json.Serialization;

namespace Domain;

public sealed record Developer
{
    public long Id { get; set; }
    public string Name { get; set; }
    [JsonIgnore]
    public IEnumerable<Game> Games { get; set; }
}
