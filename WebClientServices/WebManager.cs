namespace WebManagers;

public abstract class WebManager
{
    public WebManager(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    public HttpClient HttpClient { get; }
}
