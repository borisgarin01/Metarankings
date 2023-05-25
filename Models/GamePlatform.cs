using System.Reflection.Metadata.Ecma335;

namespace Metarankings.Models
{
    public class GamePlatform
    {
        public long Id { get; set; }
        public Game Game { get; set; }
        public long GameId { get; set; }
        public Platform Platform { get; set; }
        public long PlatformId { get; set; }
    }
}
