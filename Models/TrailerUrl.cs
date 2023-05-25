namespace Metarankings.Models
{
    public class TrailerUrl
    {
        public long Id { get; set; }
        public Game Game { get; set; }
        public long GameId { get; set; }
        public string Url { get; set; }
    }
}
