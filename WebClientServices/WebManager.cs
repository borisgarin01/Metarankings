namespace WebManagers;

public abstract class WebManager
{
    public const string AuthorizedClientName = "AuthorizedClient";

    public WebManager(IHttpClientFactory httpClientFactory)
    {
        HttpClientFactory = httpClientFactory;
    }

    public IHttpClientFactory HttpClientFactory { get; }

    protected HttpClient Client => HttpClientFactory.CreateClient(AuthorizedClientName);
}
