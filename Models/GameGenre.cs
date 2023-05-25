namespace Metarankings.Models
{
    public class GameGenre
    {
        public long Id { get; set; }
        public long GameId { get; set; }
        public Game Game { get; set; }
        public Genre Genre { get; set; }
        public long GenreId { get; set; }
    }
}
