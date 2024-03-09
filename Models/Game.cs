using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Strings;

namespace Metarankings.Models
{
    [PublishedModel("game")]
    public class Game : PublishedContentModel
    {
        public Game(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        {
        }
        public BaseDetailsPageModel BaseDetailsPageModel => this.Value<IPublishedContent>("baseDetailsPageModel") as BaseDetailsPageModel;
        public string GameName => this.Value<string>("gameName");
        public float Rating => this.Value<float>("rating");
        public IEnumerable<Link> Developers => this.Value<IEnumerable<Link>>("developers");
        public Link Publisher => this.Value<IEnumerable<Link>>("publisher").First();
        public IEnumerable<Link> Platforms => this.Value<IEnumerable<Link>>("platforms") == null ? Array.Empty<Link>() : this.Value<IEnumerable<Link>>("platforms");
        public IEnumerable<Link> Genres => this.Value<IEnumerable<Link>>("genres") == null ? Array.Empty<Link>() : this.Value<IEnumerable<Link>>("genres");
        public Link Localization => this.Value<IEnumerable<Link>>("localization").First();
        public DateTime ReleaseDate => this.Value<DateTime>("releaseDate");
        public string Description => this.Value<string>("description");
        public string ImageSource => this.Value<string>("image");
        public IEnumerable<string> Tags => this.Value<IEnumerable<string>>("tags") == null ? Array.Empty<string>(): this.Value<IEnumerable<string>>("tags");
        public Link BestGamesOfThisGameReleaseYear => this.Value<IEnumerable<Link>>("bestGamesOfThisGameReleaseYear").First();
        public Link BestGamesOfPreviousYearForGameReleaseYear => this.Value<IEnumerable<Link>>("bestGamesOfPreviousYearForGameReleaseYear").First();
        public Link NewGames => this.Value<IEnumerable<Link>>("newGames").First();
        public Link GamesReleaseDate => this.Value<IEnumerable<Link>>("gamesReleaseDate").First();
        public IEnumerable<GameJournalistsCompanyReviewModel> GameJournalistsCompanyReviewsModels => (this.Value<IEnumerable<IPublishedContent>>("gameJournalistsCompanyReviewsModels") != null) ? this.Value<IEnumerable<IPublishedContent>>("gameJournalistsCompanyReviewsModels").Select(a => a as GameJournalistsCompanyReviewModel) : Enumerable.Empty<GameJournalistsCompanyReviewModel>();
        public IEnumerable<GamePlayerReviewModel> GamePlayersReviewsModels => (this.Value<IEnumerable<IPublishedContent>>("gamePlayersReviewsModels") != null) ? this.Value<IEnumerable<IPublishedContent>>("gamePlayersReviewsModels").Select(a => a as GamePlayerReviewModel) : Enumerable.Empty<GamePlayerReviewModel>();

        public IEnumerable<Link> RightSideGamesLinks => this.Value<IEnumerable<Link>>("rightSideGamesLinks") != null ? this.Value<IEnumerable<Link>>("rightSideGamesLinks").Select(a => a as Link) : Enumerable.Empty<Link>();
        //TODO: make trailers and screenshots customizable
    }
}
