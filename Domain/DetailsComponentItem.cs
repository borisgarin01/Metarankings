using System.ComponentModel.DataAnnotations;

namespace Domain;

public sealed record DetailsComponentItem
{
    public long Id { get; set; }

    [MaxLength(255)]
    [MinLength(1)]
    [Required]
    public string Name { get; set; }

    [MaxLength(511)]
    [MinLength(1)]
    [Required]
    public string ImageSource { get; set; }

    [Range(0, 10)]
    public float CriticsAverageScore { get; set; }

    [Range(0, 10)]
    public float GamersAverageScore { get; set; }

    [Range(0, 10)]
    public float Metarating { get; set; }

    [Range(0, 100)]
    public float ExpectationsPercent { get; set; }
    public int MarksCount { get; set; }

    [MaxLength(511)]
    [MinLength(1)]
    public string OriginalName { get; set; }
    //public Studio Studio { get; set; }
    //public Genre[] Genres { get; set; }
    //public Director Director { get; set; }
    public DateOnly PremiereDate { get; set; }
    public string Description { get; set; }
    //public Actor[] Actors { get; set; }
    //public Trailer[] Trailers { get; set; }
    //public Screenshot[] Screenshots { get; set; }

}
