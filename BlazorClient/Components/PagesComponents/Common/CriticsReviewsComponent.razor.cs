using Domain.Reviews;
using System.Globalization;
using WebManagers.Derived.CriticsReviews;

namespace BlazorClient.Components.PagesComponents.Common;

/// <summary>
/// Блок "Оценки": рецензии критиков на игру или фильм и общий рейтинг критиков.
/// </summary>
public partial class CriticsReviewsComponent : ComponentBase
{
    private bool isLoaded;
    private long? loadedEntityId;

    [Parameter, EditorRequired]
    public long EntityId { get; set; }

    [Parameter, EditorRequired]
    public CriticReviewTarget Target { get; set; }

    [Parameter, EditorRequired]
    public string Name { get; set; } = string.Empty;

    [Parameter]
    public int? Year { get; set; }

    [Inject]
    public GamesCriticsReviewsWebManager GamesCriticsReviewsWebManager { get; set; } = default!;

    [Inject]
    public MoviesCriticsReviewsWebManager MoviesCriticsReviewsWebManager { get; set; } = default!;

    public List<CriticReview> CriticsReviews { get; private set; } = new();

    private CriticsReviewsWebManager WebManager => Target == CriticReviewTarget.Game
        ? GamesCriticsReviewsWebManager
        : MoviesCriticsReviewsWebManager;

    private string KindLabel => Target == CriticReviewTarget.Game ? "игру" : "фильм";

    private string AdminHref => $"/admin/{(Target == CriticReviewTarget.Game ? "games" : "movies")}/critics-reviews/{EntityId}";

    private float AverageScore => CriticsReviews.Count == 0 ? 0 : CriticsReviews.Average(cr => cr.Score);

    private string ReviewsWord => Plural(CriticsReviews.Count, "рецензия", "рецензии", "рецензий");

    private string ScoresWord => Plural(CriticsReviews.Count, "оценки", "оценок", "оценок");

    protected override async Task OnParametersSetAsync()
    {
        if (loadedEntityId == EntityId)
            return;

        loadedEntityId = EntityId;
        isLoaded = false;
        CriticsReviews = new();

        try
        {
            CriticsReviews = (await WebManager.GetByEntityAsync(EntityId)).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load critics reviews: {ex.Message}");
        }
        finally
        {
            isLoaded = true;
        }
    }

    public static int GetMark(float score) => (int)Math.Round(score, MidpointRounding.AwayFromZero);

    public static string FormatScore(float score) => score.ToString("0.#", CultureInfo.InvariantCulture);

    /// <summary>
    /// Склонение по числу: 1 рецензия, 2 рецензии, 5 рецензий.
    /// </summary>
    private static string Plural(int count, string one, string few, string many)
    {
        int lastTwoDigits = count % 100;
        int lastDigit = count % 10;

        if (lastTwoDigits is >= 11 and <= 14)
            return many;

        return lastDigit switch
        {
            1 => one,
            >= 2 and <= 4 => few,
            _ => many
        };
    }
}

public enum CriticReviewTarget
{
    Game,
    Movie
}
