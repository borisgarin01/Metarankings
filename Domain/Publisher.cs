using System.Text.Json.Serialization;

namespace Domain;

public class Publisher
{
    public long Id { get; set; }
    public string Name { get; set; }

    [JsonIgnore]
    public IEnumerable<Game> Games { get; set; }
}