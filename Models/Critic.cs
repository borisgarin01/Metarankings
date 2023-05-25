using System.ComponentModel.DataAnnotations;

namespace Metarankings.Models
{
    public class Critic
    {
        public long Id { get; set; }
        //TODO: MaxLength, MinLength, Required
        [MaxLength(255, ErrorMessage = "Critic name max length is 255"),MinLength(1,ErrorMessage = "Critic name min length is 1"),Required(ErrorMessage ="Critic name is required")]
        public string Name { get; set; }
    }
}
