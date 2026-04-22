namespace BlazorClient.Components.PagesComponents.Home;

public partial class CollectionsComponent : ComponentBase
{
    public IEnumerable<CollectionsItemComponent> CollectionsItemComponents { get; set; }

    protected override Task OnInitializedAsync()
    {
        CollectionsItemComponents = new CollectionsItemComponent[]
        {
            new CollectionsItemComponent
            {
                Href="https://metarankings.ru/samye-slozhnye-igry/",
                Title="Самые сложные игры",
                ImageAlt="Самые сложные игры",
                ImageSource="https://metarankings.ru/images/uploads/2023/06/slozhnye-igry-445x250.jpg"
            },
            new CollectionsItemComponent
            {
                Href="https://metarankings.ru/best-films-pro-zhenshhin/",
                Title="Лучшие фильмы про женщин",
                ImageAlt="Лучшие фильмы про женщин",
                ImageSource="https://metarankings.ru/images/uploads/2023/06/films-pro-zhenshhin-445x250.jpg"
            },
            new CollectionsItemComponent
            {
                Href="https://metarankings.ru/filmy-pro-realnye-sobytiya/",
                Title="Фильмы про реальные события",
                ImageAlt="Фильмы про реальные события",
                ImageSource="https://metarankings.ru/images/uploads/2023/06/pro-realnye-sobytiya-445x250.jpg"
            },
            new CollectionsItemComponent
            {
                Href="https://metarankings.ru/luchshie-filmy-pro-monstrov/",
                Title="Лучшие фильмы про монстров",
                ImageAlt="Лучшие фильмы про монстров",
                ImageSource="https://metarankings.ru/images/uploads/2023/06/filmy-pro-monstrov-445x250.jpg"
            },
            new CollectionsItemComponent
            {
                Href="https://metarankings.ru/samye-strashnye-igry/",
                Title="Самые страшные игры",
                ImageAlt="Самые страшные игры",
                ImageSource="https://metarankings.ru/images/uploads/2023/06/strashnye-igry-445x250.jpg"
            },
            new CollectionsItemComponent
            {
                Href="https://metarankings.ru/best-games-open-world/",
                Title="Лучшие игры с открытым миром",
                ImageAlt="Лучшие игры с открытым миром",
                ImageSource="https://metarankings.ru/images/uploads/2023/06/best-igry-s-otkrytym-mirom-445x250.jpg"
            }
        };

        return Task.CompletedTask;
    }
}
