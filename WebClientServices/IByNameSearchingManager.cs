namespace WebManagers;

public interface IByNameSearchingManager<T>
{
    public Task<IEnumerable<T>> SearchByName(string name);
}
