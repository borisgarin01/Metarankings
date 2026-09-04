namespace BlazorClient.Pages;

public partial class CarouselItem : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool IsActive { get; set; }
    [Parameter] public int Index { get; set; }
    [Parameter] public string? Caption { get; set; }
    [Parameter] public string? ImageAlt { get; set; }

    [CascadingParameter]
    private Carousel? ParentCarousel { get; set; }

    protected override void OnInitialized()
    {
        // Добавляем себя в родительскую карусель
        ParentCarousel?.AddItem(this);
    }
}
