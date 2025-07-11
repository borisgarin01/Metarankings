﻿using System.ComponentModel.DataAnnotations;

namespace API.Models.RequestsModels.Movies.MoviesDirectors;

public sealed record AddMovieDirectorModel
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Max length is 255")]
    [MinLength(1, ErrorMessage = "Name should be not empty")]
    public string Name { get; set; }
}
