using Domain.Reviews;
using ViewModels;

namespace BlazorClient.Pages;

public sealed partial class LatestReviewsComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<MovieReviewListViewModel> MoviesReviewsListViewModels { get; set; }

    [Parameter, EditorRequired]
    public IEnumerable<GameReviewListViewModel> GamesReviewsListViewModels { get; set; }
}
