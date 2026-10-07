namespace WebManagers;

internal static class QueryStringBuilder
{
    /// <summary>
    /// Строит строку запроса вида "?name=1&amp;name=2&amp;other=3" из набора идентификаторов.
    /// Пустые и null-наборы пропускаются; если параметров нет, возвращается пустая строка.
    /// </summary>
    public static string Build(params (string Name, IEnumerable<long>? Ids)[] parameters)
    {
        List<string> queryParams = parameters
            .Where(parameter => parameter.Ids is not null)
            .SelectMany(parameter => parameter.Ids!.Select(id => $"{parameter.Name}={id}"))
            .ToList();

        return queryParams.Count > 0
            ? "?" + string.Join("&", queryParams)
            : string.Empty;
    }
}
