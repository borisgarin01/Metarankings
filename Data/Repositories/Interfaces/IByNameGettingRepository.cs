using System;

public interface IByNameGettingRepository<T>
{
    public Task<T> GetByNameAsync(string name);
}
