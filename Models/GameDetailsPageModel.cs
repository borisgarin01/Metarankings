using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Strings;

namespace Metarankings.Models
{
    [PublishedModel("gameDetailsPageModel")]
    public class GameDetailsPageModel : PublishedContentModel
    {
        public GameDetailsPageModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        {
        }
        public string GameName => this.Value<string>("gameName");
        public float Rating => this.Value<float>("rating");
        public string Developer => this.Value<string>("developer");
        public string Publisher => this.Value<string>("publisher");
        public string[] Platforms => this.Value<string[]>("platforms");
        public string[] Genres => this.Value<string[]>("genres");
        public string Localization => this.Value<string>("localization");
        public DateTime ReleaseDate => this.Value<DateTime>("releaseDate");
        public IHtmlEncodedString Description => this.Value<IHtmlEncodedString>("description");
        public string ImageSource => this.Value<string>("image");
        public IEnumerable<string> Tags => this.Value<IEnumerable<string>>("tags");
        public IEnumerable<Link> MoviesKindsPagesUrls => this.Value<IEnumerable<Link>>("moviesKindsPagesUrls");
        public IEnumerable<Link> GamesKindsPagesUrls => this.Value<IEnumerable<Link>>("gamesKindsPagesUrls");
    }
}
