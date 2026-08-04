using Domain.Games;

namespace BlazorClient.Components.PagesComponents.DeveloperDetails;

public partial class OtherNumberOfDevelopersComponent : ComponentBase
{
    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    public IEnumerable<Developer> OtherNumberOfDevelopers { get; set; }

    [Parameter]
    public int DevelopersGettingOffset { get; set; }

    [Parameter]
    public int DevelopersGettingLimit { get; set; }

    protected override async Task OnInitializedAsync()
    {
        OtherNumberOfDevelopers = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Developer>>($"/api/Games//Developers/{DevelopersGettingOffset}/{DevelopersGettingLimit}");
    }
}
