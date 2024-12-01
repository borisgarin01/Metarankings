using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain;
public sealed record CollectionsComponentItem
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string ItemHref { get; set; }
    public string Title { get; set; }
    public string ImageSrc { get; set; }
    public string ImageAlt { get; set; }
    public string CategoryTitle { get; set; }
    public string CategoryHref { get; set; }
}

