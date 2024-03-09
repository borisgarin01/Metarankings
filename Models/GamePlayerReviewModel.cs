using System.ComponentModel.DataAnnotations;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Strings;

namespace Metarankings.Models
{
    [PublishedModel("gamePlayerReviewModel")]
    public class GamePlayerReviewModel : PublishedContentModel
    {
        public GamePlayerReviewModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        {
        }

        public string CommentAuthor => this.Value<string>("commentAuthor");
        public float? Score => this.Value<float?>("score");
        public DateTime PublishDate => this.Value<DateTime>("publishDate");
        public string Text => this.Value<string>("text");
    }
}
