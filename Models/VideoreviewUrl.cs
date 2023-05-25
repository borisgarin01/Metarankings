namespace Metarankings.Models
{
    public class VideoreviewUrl
    {
        public long Id { get; set; }
        public string VideoUrl { get; set; }
        public Game Game { get; set; }
        public long GameId { get; set; }
    }
}
