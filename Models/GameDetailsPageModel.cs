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
        public Link CollectionsURL => this.Value<IEnumerable<Link>>("collectionsURL").First();
        public Link ReviewsURL => this.Value<IEnumerable<Link>>("reviewsURL").First();
        public Link BestGamesOfThisGameReleaseYear => this.Value<IEnumerable<Link>>("bestGamesOfThisGameReleaseYear").First();
        public Link BestGamesOfPreviousYearForGameReleaseYear => this.Value<IEnumerable<Link>>("bestGamesOfPreviousYearForGameReleaseYear").First();
        public Link NewGames => this.Value<IEnumerable<Link>>("newGames").First();
        public Link GamesReleaseDate => this.Value<IEnumerable<Link>>("gamesReleaseDate").First();
        public Link MostExpectedGames => this.Value<IEnumerable<Link>>("mostExpectedGames").First();
        public IEnumerable<GameJournalistsCompanyReviewModel> GameReviews => this.Value<IEnumerable<IPublishedContent>>("reviews").Select(a=>a as GameJournalistsCompanyReviewModel);
    }
}
