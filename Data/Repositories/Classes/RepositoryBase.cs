namespace Data.Repositories.Classes;
public abstract class RepositoryBase<T> where T : class, new()
{
    public string ConnectionString { get; set; }

    protected RepositoryBase(string connectionString)
    {
        ConnectionString = connectionString;
    }
}
