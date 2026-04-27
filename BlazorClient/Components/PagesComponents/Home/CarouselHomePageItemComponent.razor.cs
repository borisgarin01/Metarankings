namespace BlazorClient.Components.PagesComponents.Home;

public partial class CarouselHomePageItemComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public string ImageSource { get; set; }

    [Parameter, EditorRequired]
    public string CardTitle { get; set; }

    [Parameter, EditorRequired]
    public string CardText { get; set; }
}
