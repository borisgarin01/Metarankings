using Data.Repositories.Interfaces;
using Domain.Games;

namespace Data.Repositories.Classes.Derived;

public sealed class TagsRepository : Repository<Tag, AddTagModel, UpdateTagModel>
{
    public TagsRepository(string connectionString) : base(connectionString)
    {
    }

    public override Task<long> AddAsync(AddTagModel entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task AddRangeAsync(IEnumerable<AddTagModel> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<Tag>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<Tag> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<Tag>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveRangeAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<Tag> UpdateAsync(UpdateTagModel entity, long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
