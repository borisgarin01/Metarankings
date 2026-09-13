namespace BlazorClient.Models;

public sealed record SlideItem(
    string Url,
    string Title,
    string Image,
    float? Score);