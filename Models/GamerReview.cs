namespace Metarankings.Models
{
    public class GamerReview
    {
        public long Id { get; set; }
        public long GamerId { get; set; }
        public Gamer Gamer { get; set; }
        public long GameId { get; set; }
        public Game Game { get; set; }
        public string Text { get; set; }
        public float Score { get; set; }
    }
}
