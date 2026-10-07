using Data.Repositories.Interfaces;
using Domain.Games;

namespace Data.Repositories.Classes.Derived;

public sealed class TagsRepository : Repository<Tag, AddTagModel, UpdateTagModel>
{
    public TagsRepository(string connectionString) : base(connectionString)
    {
    }

    public override Task<long> AddAsync(AddTagModel entity)
    {
        throw new NotImplementedException();
    }

    public override Task AddRangeAsync(IEnumerable<AddTagModel> entities)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<Tag>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public override Task<Tag> GetAsync(long id)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<Tag>> GetAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveAsync(long id)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        throw new NotImplementedException();
    }

    public override Task<Tag> UpdateAsync(UpdateTagModel entity, long id)
    {
        throw new NotImplementedException();
    }
}
