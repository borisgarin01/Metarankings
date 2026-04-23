
namespace BlazorClient.Components.PagesComponents.Home;

public partial class SoonAtCinemasComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<SoonAtCinemasItemComponent> SoonAtCinemasItemComponents { get; set; }
}
