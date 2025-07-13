using Data.Repositories.Interfaces;
using Domain.Games;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories.Classes.Derived;

public sealed class TagsRepository : Repository, IRepository<Tag>
{
    public TagsRepository(string connectionString) : base(connectionString)
    {
    }

    public Task<long> AddAsync(Tag entity)
    {
        throw new NotImplementedException();
    }

    public Task AddRangeAsync(IEnumerable<Tag> entities)
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

    public Task<Tag> UpdateAsync(Tag entity, long id)
    {
        throw new NotImplementedException();
    }
}
