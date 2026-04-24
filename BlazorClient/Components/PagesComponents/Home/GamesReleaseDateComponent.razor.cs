using ViewModels;

namespace BlazorClient.Components.PagesComponents.Home;

public partial class GamesReleaseDateComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<GamesReleaseDateItemViewModel> GamesReleaseDateItemComponents { get; set; }
}
