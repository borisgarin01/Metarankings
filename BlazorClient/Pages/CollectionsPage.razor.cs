using Domain.Games.Collections;
using Domain.Movies.Collections;
using Domain.RequestsModels.Games.Collections;
using Domain.RequestsModels.Movies.Collections;
using WebManagers;

namespace BlazorClient.Pages
{
    public partial class CollectionsPage : CancellableComponentBase
    {
        private IEnumerable<GamesCollection> gamesCollections;
        private IEnumerable<MoviesCollection> moviesCollections;

        [Inject]
        public IWebManager<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel> GamesCollectionsWebManager { get; set; }

        [Inject]
        public IWebManager<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel> MoviesCollectionsWebManager { get; set; }

        public IEnumerable<GamesCollection> GamesCollections
        {
            get => gamesCollections;
            set
            {
                gamesCollections = value;
                StateHasChanged();
            }
        }

        public IEnumerable<MoviesCollection> MoviesCollections
        {
            get => moviesCollections;
            set
            {
                moviesCollections = value;
                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Task<IEnumerable<GamesCollection>> gamesCollectionsGettingTask = GamesCollectionsWebManager.GetFirstAsync(0, 5, DisposalToken);
            Task<IEnumerable<MoviesCollection>> moviesCollectionsGettingTask = MoviesCollectionsWebManager.GetFirstAsync(0, 5, DisposalToken);

            await Task.WhenAll(gamesCollectionsGettingTask, moviesCollectionsGettingTask);
            GamesCollections = gamesCollectionsGettingTask.Result;
            MoviesCollections = moviesCollectionsGettingTask.Result;
        }

        public async Task DeleteCollectionAsync(long id)
        {
            await GamesCollectionsWebManager.DeleteAsync(id, DisposalToken);
            GamesCollections = await GamesCollectionsWebManager.GetAllAsync(DisposalToken);
        }
    }
}
