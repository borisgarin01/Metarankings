using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public sealed record IndexMoviesComponentItem
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string ItemHref { get; set; }
    public string Title { get; set; }
    public float Score { get; set; }
    public string Description { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string ImageSrc { get; set; }
    public string ImageAlt { get; set; }
    public Genre[] Genres { get; set; }
}

public sealed record Genre
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Name { get; set; }
    public string Href { get; set; }
}

public sealed record IndexMoviesComponentItemGenre
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public long IndexMoviesComponentItemId { get; set; }
    public IndexMoviesComponentItem IndexMoviesComponentItem { get; set; }
    public long GenreId { get; set; }
    public Genre Genre { get; set; }
}