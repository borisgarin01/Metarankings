namespace Domain;

public sealed record BestInPastMonth
{
    public string Name { get; set; }

    public string Href { get; set; }

    public float Score { get; set; }

    public string ScoreStyle { get { return $"small-score mark-{Math.Round(Score)}"; } }

    public string ImageSrc { get; set; }
}
