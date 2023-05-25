namespace Metarankings.Models
{
    public class ScreetshotUrl
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; }
        public Game Game { get; set; }
        public long GameId { get; set; }
    }
}
