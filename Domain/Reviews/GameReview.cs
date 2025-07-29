namespace Domain.Reviews;

public sealed record GameReview(
    [property: Key]
    [property:DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    long Id,

    long GameId,

    long UserId,

    [property:Required(ErrorMessage ="Text should be set")]
    [property:MinLength(1,ErrorMessage ="Text should be not empty")]
    string Text);