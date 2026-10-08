using BlazorClient.Components.PagesComponents.Common;
using Domain.RequestsModels.CriticsReviews;
using Domain.Reviews;
using System.Net;
using WebManagers.Derived.CriticsReviews;
using WebManagers.Derived.Games;
using WebManagers.Derived.Movies;

namespace BlazorClient.Pages.Admin;

/// <summary>
/// Управление рецензиями критиков игры или фильма.
/// </summary>
public partial class CriticsReviewsPage : CancellableComponentBase
{
    private bool isLoaded;
    private bool isSaving;
    private bool isError;
    private string? message;
    private long? editingId;
    private CriticReviewModel form = new();

    /// <summary>
    /// "games" или "movies".
    /// </summary>
    [Parameter]
    public string Section { get; set; } = string.Empty;

    [Parameter]
    public long EntityId { get; set; }

    [Inject]
    public GamesCriticsReviewsWebManager GamesCriticsReviewsWebManager { get; set; } = default!;

    [Inject]
    public MoviesCriticsReviewsWebManager MoviesCriticsReviewsWebManager { get; set; } = default!;

    [Inject]
    public GamesWebManager GamesWebManager { get; set; } = default!;

    [Inject]
    public MoviesWebManager MoviesWebManager { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    public string? EntityName { get; private set; }

    public List<CriticReview> CriticsReviews { get; private set; } = new();

    private CriticReviewTarget? Target => Section.ToLowerInvariant() switch
    {
        "games" => CriticReviewTarget.Game,
        "movies" => CriticReviewTarget.Movie,
        _ => null
    };

    private CriticsReviewsWebManager WebManager => Target == CriticReviewTarget.Game
        ? GamesCriticsReviewsWebManager
        : MoviesCriticsReviewsWebManager;

    private string DetailsHref => $"/{(Target == CriticReviewTarget.Game ? "games" : "movies")}/details/{EntityId}";

    private string ListHref => Target == CriticReviewTarget.Game ? "/admin/games/games/list-games" : "/admin/movies/list-movies";

    protected override async Task OnParametersSetAsync()
    {
        isLoaded = false;
        EntityName = null;
        CriticsReviews = new();
        message = null;
        ResetForm();

        if (Target is null)
            return;

        try
        {
            EntityName = Target == CriticReviewTarget.Game
                ? (await GamesWebManager.GetAsync(EntityId, DisposalToken))?.Name
                : (await MoviesWebManager.GetAsync(EntityId, DisposalToken))?.Name;

            if (EntityName is not null)
                await LoadReviewsAsync();
        }
        catch (Exception ex) when (!DisposalToken.IsCancellationRequested)
        {
            ShowError($"Ошибка загрузки: {ex.Message}");
        }
        finally
        {
            isLoaded = true;
        }
    }

    private async Task LoadReviewsAsync()
    {
        CriticsReviews = (await WebManager.GetByEntityAsync(EntityId, DisposalToken)).ToList();
    }

    private async Task SaveAsync()
    {
        isSaving = true;
        bool wasEditing = editingId is not null;
        try
        {
            form.EntityId = EntityId;

            HttpResponseMessage response = wasEditing
                ? await WebManager.UpdateAsync(editingId!.Value, form, DisposalToken)
                : await WebManager.AddAsync(form, DisposalToken);

            if (!response.IsSuccessStatusCode)
            {
                ShowError(response.StatusCode == HttpStatusCode.Conflict
                    ? "Рецензия этого издания уже добавлена."
                    : $"Не удалось сохранить: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync(DisposalToken)}");
                return;
            }

            ResetForm();
            ShowSuccess(wasEditing ? "Рецензия обновлена." : "Рецензия добавлена.");
            await LoadReviewsAsync();
        }
        catch (Exception ex) when (!DisposalToken.IsCancellationRequested)
        {
            ShowError($"Ошибка: {ex.Message}");
        }
        finally
        {
            isSaving = false;
        }
    }

    private void StartEdit(CriticReview criticReview)
    {
        editingId = criticReview.Id;
        form = new CriticReviewModel
        {
            EntityId = criticReview.EntityId,
            Publication = criticReview.Publication,
            Author = criticReview.Author,
            Score = criticReview.Score,
            TextContent = criticReview.TextContent,
            SourceUrl = criticReview.SourceUrl,
            Date = criticReview.Date
        };
        message = null;
    }

    private void CancelEdit()
    {
        ResetForm();
        message = null;
    }

    private async Task DeleteAsync(CriticReview criticReview)
    {
        if (!await JSRuntime.InvokeAsync<bool>("confirm", $"Удалить рецензию «{criticReview.Publication}»?"))
            return;

        try
        {
            HttpResponseMessage response = await WebManager.DeleteAsync(criticReview.Id, DisposalToken);
            if (!response.IsSuccessStatusCode)
            {
                ShowError($"Не удалось удалить: {(int)response.StatusCode}");
                return;
            }

            if (editingId == criticReview.Id)
                ResetForm();

            ShowSuccess("Рецензия удалена.");
            await LoadReviewsAsync();
        }
        catch (Exception ex) when (!DisposalToken.IsCancellationRequested)
        {
            ShowError($"Ошибка удаления: {ex.Message}");
        }
    }

    private void ResetForm()
    {
        editingId = null;
        form = new CriticReviewModel { EntityId = EntityId };
    }

    private void ShowSuccess(string text)
    {
        isError = false;
        message = text;
    }

    private void ShowError(string text)
    {
        isError = true;
        message = text;
    }
}
