using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Metarankings.Models
{
    [PublishedModel("baseDetailsPageModel")]
    public class BaseDetailsPageModel : PublishedContentModel
    {
        public BaseDetailsPageModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        {
        }

        public IEnumerable<Link> MoviesKindsPagesUrls => this.Value<IEnumerable<Link>>("moviesKindsPagesUrls"); 
        public Link MostExpectedGames => this.Value<IEnumerable<Link>>("mostExpectedGames").First();
        public IEnumerable<Link> GamesKindsPagesUrls => this.Value<IEnumerable<Link>>("gamesKindsPagesUrls");
        public Link CollectionsURL => this.Value<IEnumerable<Link>>("collectionsURL").First();
        public Link ReviewsURL => this.Value<IEnumerable<Link>>("reviewsURL").First();
    }
}
