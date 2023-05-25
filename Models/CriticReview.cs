using System.ComponentModel.DataAnnotations;

namespace Metarankings.Models
{
    public class CriticReview
    {
        public long Id { get; set; }
        public long CriticId { get; set; }
        public Critic Critic { get; set; }
        public long GameId { get; set; }
        public Game Game { get; set; }
        [MaxLength(16383, ErrorMessage = "Text max length is 16383"), MinLength(1, ErrorMessage = "Text min length is 1"), Required(ErrorMessage = "Critic name is required")]
        public string Text { get; set; }
        [Range(0.0,10.0)]
        public float Score { get; set; }
    }
}
