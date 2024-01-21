using Umbraco.Cms.Core.Models.PublishedContent;

namespace Metarankings.Models
{
    [PublishedModel("album")]
    public class Album : PublishedContentModel
    {
        public Album(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        {
        }
        public string Title => this.Value<string>("title");
        public IEnumerable<Song> Songs => this.Children<Song>();
        public DateTime ReleaseDate => this.Value<DateTime>("releaseDate");
    }
}