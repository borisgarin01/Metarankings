using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameTags : ComponentBase
{
    [Parameter]
    public List<string> Links { get; set; } = new();

    private string GetLinkText(string link)
    {
        return link switch
        {
            string l when l.Contains("best-ps4-games") => "Топ игры для PS4",
            string l when l.Contains("priklyucheniya") => "приключения",
            _ => "Ссылка"
        };
    }
}
