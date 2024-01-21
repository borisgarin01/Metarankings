using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Strings;

namespace Metarankings.Models
{
    [PublishedModel("song")]
    public class Song : PublishedContentModel
    {
        public Song(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        {
        }
        public string Title => this.Value<string>("title");
        public IHtmlEncodedString Lyrics => this.Value<IHtmlEncodedString>("lyrics");
    }
}
