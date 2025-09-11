using Domain.Games;
using Microsoft.AspNetCore.Http;

namespace WebManagers;

public interface IWebManager<T, TAdd, TUpdate> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(long offset, long limit);
    Task<T> GetAsync(long id);
    Task<HttpResponseMessage> DeleteAsync(long id);
    Task<T> UpdateAsync(long id, TUpdate tUpdate);
    Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<TAdd> adds);
    Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile);
    Task<HttpResponseMessage> AddAsync(TAdd tAdd);
}
