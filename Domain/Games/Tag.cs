namespace Domain.Games;

public sealed record Tag
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required(ErrorMessage = "Title is requeried")]
    [MaxLength(255, ErrorMessage = "Title's max length is 255")]
    [MinLength(1, ErrorMessage = "Title should be not empty")]
    public string Title { get; set; }
}
