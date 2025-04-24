namespace Data.Repositories.Classes;
public abstract class Repository
{
    public string ConnectionString { get; }

    public Repository(string connectionString)
    {
        ConnectionString = connectionString;
    }
}
