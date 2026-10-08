using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace WebManagers;

/// <summary>
/// Базовый клиент для REST-ресурса со стандартным набором CRUD-эндпоинтов:
/// GET {base}, GET {base}/{id}, GET {base}/{offset}/{limit}, POST {base}, PUT {base}/{id}, DELETE {base}/{id}.
/// Нестандартные эндпоинты переопределяются в наследниках.
/// </summary>
public abstract class CrudWebManager<T, TAdd, TUpdate> : WebManager, IWebManager<T, TAdd, TUpdate> where T : class
{
    protected CrudWebManager(IHttpClientFactory httpClientFactory, string basePath) : base(httpClientFactory)
    {
        BasePath = basePath;
    }

    protected string BasePath { get; }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Client.GetFromJsonAsync<IEnumerable<T>>(BasePath, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetFirstAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        return await Client.GetFromJsonAsync<IEnumerable<T>>($"{BasePath}/{offset}/{limit}", cancellationToken);
    }

    public virtual Task<IEnumerable<T>> GetLastAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<T> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        return await Client.GetFromJsonAsync<T>($"{BasePath}/{id}", cancellationToken);
    }

    public virtual Task<HttpResponseMessage> AddAsync(TAdd tAdd, CancellationToken cancellationToken = default)
    {
        return Client.PostAsJsonAsync(BasePath, tAdd, cancellationToken);
    }

    public virtual Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<TAdd> adds, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public virtual Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<T> UpdateAsync(long id, TUpdate tUpdate, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await Client.PutAsJsonAsync($"{BasePath}/{id}", tUpdate, cancellationToken);

        if (!httpResponseMessage.IsSuccessStatusCode)
            return null;

        return await httpResponseMessage.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    public virtual Task<HttpResponseMessage> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        return Client.DeleteAsync($"{BasePath}/{id}", cancellationToken);
    }

    protected Task<HttpResponseMessage> PostAsync<TBody>(string relativePath, TBody body, CancellationToken cancellationToken = default)
    {
        return Client.PostAsJsonAsync($"{BasePath}/{relativePath}", body, cancellationToken);
    }
}
