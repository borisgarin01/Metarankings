namespace BlazorClient.Models;

public sealed record SlideGroup(
    string PagerTitle,
    IEnumerable<SlideItem> Items);
