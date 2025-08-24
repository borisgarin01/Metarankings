using Domain.Games;
using Domain.RequestsModels.Games.Publishers;
using Microsoft.AspNetCore.Http;

namespace WebManagers.Derived
{
    public sealed class PublishersWebManager : WebManager, IWebManager<Publisher, AddPublisherModel, UpdatePublisherModel>
    {
        public PublishersWebManager(HttpClient httpClient) : base(httpClient)
        {
        }

        public Task<HttpResponseMessage> AddAsync(AddPublisherModel tAdd)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddPublisherModel> adds)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Publisher>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Publisher>> GetAllAsync(long offset, long limit)
        {
            throw new NotImplementedException();
        }

        public Task<Publisher> GetAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<Publisher> UpdateAsync(long id, UpdatePublisherModel tUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
