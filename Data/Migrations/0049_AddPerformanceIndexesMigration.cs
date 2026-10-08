namespace Data.Migrations;

/// <summary>
/// Индексы под реальные запросы репозиториев. PostgreSQL не создаёт индексы на внешние ключи
/// автоматически, поэтому JOIN-ы по таблицам связей, подзапросы оценок и каскадные удаления
/// без них выполнялись полным сканированием. Индексы, уже покрытые UNIQUE-ограничениями
/// (ведущая колонка), не дублируются.
/// </summary>
[Migration(49, "Add performance indexes migration")]
public sealed class AddPerformanceIndexesMigration : Migration
{
    private static readonly (string Name, string Definition)[] Indexes =
    {
        // Игры: сортировка/фильтр по дате выхода (GetLastAsync, GetNearest*, ожидаемые), фильтр по локализации
        ("IX_Games_ReleaseDate", "Games(ReleaseDate)"),
        ("IX_Games_LocalizationId", "Games(LocalizationId)"),
        // Поиск по подстроке: Name ILIKE '%...%'
        ("IX_Games_Name_Trgm", "Games USING gin (Name gin_trgm_ops)"),

        // Связи игр: GameId покрыт UNIQUE(GameId, X), нужен обратный индекс для фильтров и страниц X
        ("IX_GamesPlatforms_PlatformId", "GamesPlatforms(PlatformId, GameId)"),
        ("IX_GamesDevelopers_DeveloperId", "GamesDevelopers(DeveloperId, GameId)"),
        ("IX_GamesGenres_GenreId", "GamesGenres(GenreId, GameId)"),
        // У GamesPublishers нет ни одного индекса
        ("IX_GamesPublishers_GameId", "GamesPublishers(GameId)"),
        ("IX_GamesPublishers_PublisherId", "GamesPublishers(PublisherId, GameId)"),
        ("IX_GamesScreenshots_GameId", "GamesScreenshots(GameId)"),
        ("IX_GamesTags_GameId", "GamesTags(GameId)"),
        ("IX_GamesTags_TagId", "GamesTags(TagId)"),
        // UNIQUE(GameCollectionId, GameId) не помогает JOIN-у по GameId
        ("IX_GamesCollectionsItems_GameId", "GamesCollectionsItems(GameId)"),

        // Отзывы игроков: выборка по пользователю; голоса: выборка по ShifterId
        ("IX_GamesPlayersReviews_UserId", "GamesPlayersReviews(UserId)"),
        ("IX_GamesPlayersReviewsShifts_ShifterId", "GamesPlayersReviewsShifts(ShifterId)"),

        // Фильмы
        ("IX_Movies_PremierDate", "Movies(PremierDate)"),
        ("IX_Movies_Name_Trgm", "Movies USING gin (Name gin_trgm_ops)"),

        // Связи фильмов создавались без индексов и без UNIQUE
        ("IX_MoviesMoviesGenres_MovieId", "MoviesMoviesGenres(MovieId)"),
        ("IX_MoviesMoviesGenres_MovieGenreId", "MoviesMoviesGenres(MovieGenreId, MovieId)"),
        ("IX_MoviesMoviesStudios_MovieId", "MoviesMoviesStudios(MovieId)"),
        ("IX_MoviesMoviesStudios_MovieStudioId", "MoviesMoviesStudios(MovieStudioId, MovieId)"),
        ("IX_MoviesMoviesDirectors_MovieId", "MoviesMoviesDirectors(MovieId)"),
        ("IX_MoviesMoviesDirectors_MovieDirectorId", "MoviesMoviesDirectors(MovieDirectorId, MovieId)"),
        ("IX_MoviesTags_MovieId", "MoviesTags(MovieId)"),
        ("IX_MoviesTags_TagId", "MoviesTags(TagId)"),
        ("IX_MoviesCollectionsItems_MovieId", "MoviesCollectionsItems(MovieId)"),

        // UNIQUE(ViewerId, MovieId) не помогает подзапросам AVG/COUNT по MovieId
        ("IX_ViewersMoviesReviews_MovieId", "ViewersMoviesReviews(MovieId)"),
        ("IX_ViewersMoviesReviewsShifts_ShifterId", "ViewersMoviesReviewsShifts(ShifterId)"),

        // Новости: ORDER BY PublishTimestamp DESC
        ("IX_News_PublishTimestamp", "News(PublishTimestamp DESC)"),

        // Identity: поиск пользователя при каждом входе/регистрации
        ("IX_ApplicationUsers_NormalizedUserName", "ApplicationUsers(NormalizedUserName)"),
        ("IX_ApplicationUsers_NormalizedEmail", "ApplicationUsers(NormalizedEmail)"),
        ("IX_ApplicationUsersRoles_RoleId", "ApplicationUsersRoles(RoleId)"),
        ("IX_AspNetUserLogins_UserId", "AspNetUserLogins(UserId)"),
        ("IX_AccessTokens_UserId_LoginProvider_Name", "AccessTokens(UserId, LoginProvider, Name)"),

        // Refresh-токены: поиск по значению при каждом обновлении, список токенов пользователя
        ("IX_RefreshTokens_Value", "RefreshTokens(Value)"),
        ("IX_RefreshTokens_UserId_CreatedAt", "RefreshTokens(UserId, CreatedAt DESC)"),
    };

    public override void Up()
    {
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");

        foreach ((string name, string definition) in Indexes)
            Execute.Sql($"CREATE INDEX IF NOT EXISTS {name} ON {definition};");
    }

    public override void Down()
    {
        foreach ((string name, _) in Indexes)
            Execute.Sql($"DROP INDEX IF EXISTS {name};");
    }
}
