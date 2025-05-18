using Domain;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Components.PagesComponents.DeveloperDetails;

public partial class OtherNumberOfDevelopersComponent : ComponentBase
{
    [Inject]
    public HttpClient HttpClient { get; set; }

    public IEnumerable<Developer> OtherNumberOfDevelopers { get; set; }

    [Parameter]
    public int DevelopersGettingOffset { get; set; }

    [Parameter]
    public int DevelopersGettingLimit { get; set; }

    protected override async Task OnInitializedAsync()
    {
        OtherNumberOfDevelopers = await HttpClient.GetFromJsonAsync<IEnumerable<Developer>>($"/api/Developers/{DevelopersGettingOffset}/{DevelopersGettingLimit}");
    }
}
