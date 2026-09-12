using Microsoft.AspNetCore.Components;

namespace BlazorClient.Pages;

public partial class CarouselItem : ComponentBase, IDisposable
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool IsActive { get; set; }
    [Parameter] public int Index { get; set; }
    [Parameter] public string? Caption { get; set; }
    [Parameter] public string? ImageAlt { get; set; }

    [CascadingParameter]
    private BlazorClient.Pages.Carousel? ParentCarousel { get; set; }

    protected override void OnInitialized()
    {
        ParentCarousel?.AddItem(this);
    }

    public void Dispose()
    {
        ParentCarousel?.RemoveItem(this);
    }
}