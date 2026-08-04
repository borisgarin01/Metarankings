namespace WebManagers;

public abstract class WebManager
{
    public WebManager(IHttpClientFactory httpClientFactory)
    {
        HttpClientFactory = httpClientFactory;
    }

    public IHttpClientFactory HttpClientFactory { get; }
}
