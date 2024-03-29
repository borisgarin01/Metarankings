﻿using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Strings;

namespace Metarankings.Models
{
    [PublishedModel("gameJournalistsCompanyReviewModel")]
    public class GameJournalistsCompanyReviewModel : PublishedContentModel
    {
        public GameJournalistsCompanyReviewModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        {
        }

        public Game GameDetailsPageModel => this.Value<Game>("gameDetailsPageModel");
        public float? Score => this.Value<float?>("score");
        public string JournalistsCompanyName => this.Value<string>("journalistsCompanyName");
        public string AuthorName => this.Value<string>("authorName");
        public string Text => this.Value<string>("text");
    }
}
