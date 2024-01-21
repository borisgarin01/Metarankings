using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Strings;

namespace Metarankings.Models
{
    [PublishedModel("band")]
    public class Band : PublishedContentModel
    {
        public Band(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        {
        }
        public string BandName => this.Value<string>("bandName");
        public IEnumerable<Song> Songs => this.Children<Song>();
        public IEnumerable<Album> Albums => this.Children<Album>();
    }
}
