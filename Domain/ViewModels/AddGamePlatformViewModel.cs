﻿using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels;
public sealed record AddGamePlatformViewModel
{
    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Name { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Url { get; set; }
}