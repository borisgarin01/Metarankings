namespace BlazorClient.Components.PagesComponents.Common;

public partial class PremierDateComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public DateTime PremierDate { get; set; }
}
