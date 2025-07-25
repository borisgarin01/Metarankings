﻿namespace Domain.Movies;
public sealed record MovieGenre
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name max length is 255")]
    [MinLength(1, ErrorMessage = "Name should be not empty")]
    public string Name { get; set; }
}
