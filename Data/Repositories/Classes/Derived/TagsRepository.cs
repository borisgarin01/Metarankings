using Data.Repositories.Interfaces;
using Domain.Games;

namespace Data.Repositories.Classes.Derived;

public sealed class TagsRepository : Repository, IRepository<Tag, AddTagModel, UpdateTagModel>
{
    public TagsRepository(string connectionString) : base(connectionString)
    {
    }

    public Task<long> AddAsync(AddTagModel entity)
    {
        throw new NotImplementedException();
    }

    public Task AddRangeAsync(IEnumerable<AddTagModel> entities)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Tag>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Tag> GetAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Tag>> GetAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        throw new NotImplementedException();
    }

    public Task<Tag> UpdateAsync(UpdateTagModel entity, long id)
    {
        throw new NotImplementedException();
    }
}
