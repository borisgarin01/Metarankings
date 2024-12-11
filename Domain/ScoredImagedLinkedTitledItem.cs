namespace Domain;
public sealed record ScoredImagedLinkedTitledItem
{
    public string Url { get; set; }
    public string ImageSource { get; set; }
    public float Score { get; set; }
    public string Title { get; set; }
}
