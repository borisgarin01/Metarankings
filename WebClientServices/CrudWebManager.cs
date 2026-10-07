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

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Client.GetFromJsonAsync<IEnumerable<T>>(BasePath);
    }

    public virtual async Task<IEnumerable<T>> GetFirstAsync(long offset, long limit)
    {
        return await Client.GetFromJsonAsync<IEnumerable<T>>($"{BasePath}/{offset}/{limit}");
    }

    public virtual Task<IEnumerable<T>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<T> GetAsync(long id)
    {
        return await Client.GetFromJsonAsync<T>($"{BasePath}/{id}");
    }

    public virtual Task<HttpResponseMessage> AddAsync(TAdd tAdd)
    {
        return Client.PostAsJsonAsync(BasePath, tAdd);
    }

    public virtual Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<TAdd> adds)
    {
        throw new NotImplementedException();
    }

    public virtual Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<T> UpdateAsync(long id, TUpdate tUpdate)
    {
        HttpResponseMessage httpResponseMessage = await Client.PutAsJsonAsync($"{BasePath}/{id}", tUpdate);

        if (!httpResponseMessage.IsSuccessStatusCode)
            return null;

        return await httpResponseMessage.Content.ReadFromJsonAsync<T>();
    }

    public virtual Task<HttpResponseMessage> DeleteAsync(long id)
    {
        return Client.DeleteAsync($"{BasePath}/{id}");
    }

    protected Task<HttpResponseMessage> PostAsync<TBody>(string relativePath, TBody body)
    {
        return Client.PostAsJsonAsync($"{BasePath}/{relativePath}", body);
    }
}
