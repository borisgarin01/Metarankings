using Microsoft.AspNetCore.Http;

namespace WebManagers;

public interface IWebManager<T, TAdd, TUpdate> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetFirstAsync(long offset, long limit, CancellationToken cancellationToken = default);
    Task<T> GetAsync(long id, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> DeleteAsync(long id, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(long id, TUpdate tUpdate, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<TAdd> adds, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> AddAsync(TAdd tAdd, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetLastAsync(long offset, long limit, CancellationToken cancellationToken = default);
}
