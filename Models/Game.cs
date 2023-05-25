using System.ComponentModel.DataAnnotations;

namespace Metarankings.Models
{
    public class Game
    {
        public long Id { get; set; }
        [MaxLength(255, ErrorMessage = "Name max length is 255"), MinLength(1, ErrorMessage = "Name min length is 1"), Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Range(0.0,10.0)]
        public float Rating { get; set; }
        public Developer Developer { get; set; }
        public long DeveloperId { get; set; }
        public Publisher Publisher { get; set; }
        public long PublisherId { get; set; }
        public string Url { get; set; }
        public Localization Localization { get; set; }
        public long LocalizationUrl { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
