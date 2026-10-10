namespace API.Caching;

/// <summary>
/// Теги output-кэша. Данные сильно связаны (игра содержит жанры, платформы, рецензии и т.п.),
/// поэтому теги крупные: любое изменение в разделе сбрасывает весь кэш этого раздела.
/// </summary>
public static class CacheTags
{
    public const string Games = "games";
    public const string Movies = "movies";
    public const string News = "news";
}
