using Domain.Games;
using Microsoft.AspNetCore.Http;

namespace WebManagers;

public interface IWebManager<T, TAdd, TUpdate> where T : class, new()
{
    Task<IEnumerable<Developer>> GetAllAsync();
    Task<IEnumerable<Developer>> GetAllAsync(long offset, long limit);
    Task<IEnumerable<Developer>> GetAsync(long id);
    Task<IEnumerable<Developer>> DeleteAsync(long id);
    Task<IEnumerable<Developer>> UpdateAsync(long id, TUpdate tUpdate);
    Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<TAdd> adds);
    Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile);
    Task<HttpResponseMessage> AddAsync(TAdd tAdd);
}
