using Domain;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.PublisherDetails;

public partial class PublisherGameComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public Game Game { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }
}
