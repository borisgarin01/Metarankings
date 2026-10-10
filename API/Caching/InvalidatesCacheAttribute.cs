namespace API.Caching;

/// <summary>
/// Изменяющие запросы (POST/PUT/PATCH/DELETE) контроллера сбрасывают кэш с указанными тегами.
/// Нужен там, где сам контроллер не кэшируется, но влияет на кэшируемые данные (например, ожидания влияют на «самые ожидаемые»).
/// Теги из [OutputCache] сбрасываются и без этого атрибута — см. <see cref="OutputCacheInvalidationFilter"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class InvalidatesCacheAttribute : Attribute
{
    public InvalidatesCacheAttribute(params string[] tags)
    {
        Tags = tags;
    }

    public IReadOnlyList<string> Tags { get; }
}
