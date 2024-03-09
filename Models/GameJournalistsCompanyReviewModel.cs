using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Strings;

namespace Metarankings.Models
{
    [PublishedModel("gameJournalistsCompanyReviewModel")]
    public class GameJournalistsCompanyReviewModel : PublishedContentModel
    {
        public GameJournalistsCompanyReviewModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        {
        }

        public GameDetailsPageModel GameDetailsPageModel => this.Value<GameDetailsPageModel>("gameDetailsPageModel");
        public float? Score => this.Value<float?>("score");
        public string JournalistsCompanyName => this.Value<string>("journalistsCompanyName");
        public string AuthorName => this.Value<string>("authorName");
        public IHtmlEncodedString Text => this.Value<IHtmlEncodedString>("text");
    }
}
