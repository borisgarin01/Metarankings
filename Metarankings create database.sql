IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Collections] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_Collections] PRIMARY KEY ([Id])
);

CREATE TABLE [Critics] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(255) NOT NULL,
    [Url] nvarchar(511) NOT NULL,
    CONSTRAINT [PK_Critics] PRIMARY KEY ([Id])
);

CREATE TABLE [Gamers] (
    [Id] bigint NOT NULL IDENTITY,
    [Url] nvarchar(255) NOT NULL,
    [AccountName] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_Gamers] PRIMARY KEY ([Id])
);

CREATE TABLE [GamesDevelopers] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(255) NOT NULL,
    [Url] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_GamesDevelopers] PRIMARY KEY ([Id])
);

CREATE TABLE [GamesGenres] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(255) NOT NULL,
    [Url] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_GamesGenres] PRIMARY KEY ([Id])
);

CREATE TABLE [GamesLocalizations] (
    [Id] bigint NOT NULL IDENTITY,
    [Url] nvarchar(255) NOT NULL,
    [Title] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_GamesLocalizations] PRIMARY KEY ([Id])
);

CREATE TABLE [GamesPlatforms] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(255) NOT NULL,
    [Url] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_GamesPlatforms] PRIMARY KEY ([Id])
);

CREATE TABLE [GamesPublishers] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(255) NOT NULL,
    [Url] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_GamesPublishers] PRIMARY KEY ([Id])
);

CREATE TABLE [GamesTags] (
    [Id] bigint NOT NULL IDENTITY,
    [Title] nvarchar(255) NOT NULL,
    [Url] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_GamesTags] PRIMARY KEY ([Id])
);

CREATE TABLE [CollectionItems] (
    [Id] bigint NOT NULL IDENTITY,
    [Href] nvarchar(255) NOT NULL,
    [Title] nvarchar(255) NOT NULL,
    [ImageSrc] nvarchar(255) NOT NULL,
    [CollectionId] bigint NOT NULL,
    CONSTRAINT [PK_CollectionItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CollectionItems_Collections_CollectionId] FOREIGN KEY ([CollectionId]) REFERENCES [Collections] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Games] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(255) NOT NULL,
    [Score] real NOT NULL,
    [ImageSource] nvarchar(255) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [LocalizationId] bigint NOT NULL,
    [ReleaseDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Games] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Games_GamesLocalizations_LocalizationId] FOREIGN KEY ([LocalizationId]) REFERENCES [GamesLocalizations] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [GameGameDeveloper] (
    [DevelopersId] bigint NOT NULL,
    [GamesId] bigint NOT NULL,
    CONSTRAINT [PK_GameGameDeveloper] PRIMARY KEY ([DevelopersId], [GamesId]),
    CONSTRAINT [FK_GameGameDeveloper_GamesDevelopers_DevelopersId] FOREIGN KEY ([DevelopersId]) REFERENCES [GamesDevelopers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GameGameDeveloper_Games_GamesId] FOREIGN KEY ([GamesId]) REFERENCES [Games] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [GameGameGenre] (
    [GamesId] bigint NOT NULL,
    [GenresId] bigint NOT NULL,
    CONSTRAINT [PK_GameGameGenre] PRIMARY KEY ([GamesId], [GenresId]),
    CONSTRAINT [FK_GameGameGenre_GamesGenres_GenresId] FOREIGN KEY ([GenresId]) REFERENCES [GamesGenres] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GameGameGenre_Games_GamesId] FOREIGN KEY ([GamesId]) REFERENCES [Games] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [GameGamePlatform] (
    [GamesId] bigint NOT NULL,
    [PlatformsId] bigint NOT NULL,
    CONSTRAINT [PK_GameGamePlatform] PRIMARY KEY ([GamesId], [PlatformsId]),
    CONSTRAINT [FK_GameGamePlatform_GamesPlatforms_PlatformsId] FOREIGN KEY ([PlatformsId]) REFERENCES [GamesPlatforms] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GameGamePlatform_Games_GamesId] FOREIGN KEY ([GamesId]) REFERENCES [Games] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [GameGamePublisher] (
    [GamesId] bigint NOT NULL,
    [PublishersId] bigint NOT NULL,
    CONSTRAINT [PK_GameGamePublisher] PRIMARY KEY ([GamesId], [PublishersId]),
    CONSTRAINT [FK_GameGamePublisher_GamesPublishers_PublishersId] FOREIGN KEY ([PublishersId]) REFERENCES [GamesPublishers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GameGamePublisher_Games_GamesId] FOREIGN KEY ([GamesId]) REFERENCES [Games] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [GameGamerReview] (
    [Id] bigint NOT NULL IDENTITY,
    [GamerId] bigint NOT NULL,
    [GameId] bigint NOT NULL,
    [Text] nvarchar(450) NOT NULL,
    [Score] real NOT NULL,
    [Url] nvarchar(450) NOT NULL,
    [ReviewDate] datetime2 NOT NULL,
    CONSTRAINT [PK_GameGamerReview] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GameGamerReview_Gamers_GamerId] FOREIGN KEY ([GamerId]) REFERENCES [Gamers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GameGamerReview_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [Games] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [GameGameTag] (
    [GamesId] bigint NOT NULL,
    [TagsId] bigint NOT NULL,
    CONSTRAINT [PK_GameGameTag] PRIMARY KEY ([GamesId], [TagsId]),
    CONSTRAINT [FK_GameGameTag_GamesTags_TagsId] FOREIGN KEY ([TagsId]) REFERENCES [GamesTags] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GameGameTag_Games_GamesId] FOREIGN KEY ([GamesId]) REFERENCES [Games] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [GamesCriticsReviews] (
    [Id] bigint NOT NULL IDENTITY,
    [CriticId] bigint NOT NULL,
    [GameId] bigint NOT NULL,
    [Text] nvarchar(450) NOT NULL,
    [Score] real NOT NULL,
    [Url] nvarchar(511) NOT NULL,
    [ReviewDate] datetime2 NOT NULL,
    CONSTRAINT [PK_GamesCriticsReviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GamesCriticsReviews_Critics_CriticId] FOREIGN KEY ([CriticId]) REFERENCES [Critics] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GamesCriticsReviews_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [Games] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Trailers] (
    [Id] bigint NOT NULL IDENTITY,
    [Url] nvarchar(450) NOT NULL,
    [GameId] bigint NOT NULL,
    CONSTRAINT [PK_Trailers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Trailers_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [Games] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_CollectionItems_CollectionId] ON [CollectionItems] ([CollectionId]);

CREATE UNIQUE INDEX [IX_CollectionItems_Href] ON [CollectionItems] ([Href]);

CREATE UNIQUE INDEX [IX_CollectionItems_ImageSrc] ON [CollectionItems] ([ImageSrc]);

CREATE UNIQUE INDEX [IX_CollectionItems_Title] ON [CollectionItems] ([Title]);

CREATE UNIQUE INDEX [IX_Collections_Name] ON [Collections] ([Name]);

CREATE UNIQUE INDEX [IX_Critics_Name] ON [Critics] ([Name]);

CREATE UNIQUE INDEX [IX_Critics_Url] ON [Critics] ([Url]);

CREATE INDEX [IX_GameGameDeveloper_GamesId] ON [GameGameDeveloper] ([GamesId]);

CREATE INDEX [IX_GameGameGenre_GenresId] ON [GameGameGenre] ([GenresId]);

CREATE INDEX [IX_GameGamePlatform_PlatformsId] ON [GameGamePlatform] ([PlatformsId]);

CREATE INDEX [IX_GameGamePublisher_PublishersId] ON [GameGamePublisher] ([PublishersId]);

CREATE INDEX [IX_GameGamerReview_GameId] ON [GameGamerReview] ([GameId]);

CREATE INDEX [IX_GameGamerReview_GamerId] ON [GameGamerReview] ([GamerId]);

CREATE UNIQUE INDEX [IX_GameGamerReview_Text] ON [GameGamerReview] ([Text]);

CREATE UNIQUE INDEX [IX_GameGamerReview_Url] ON [GameGamerReview] ([Url]);

CREATE INDEX [IX_GameGameTag_TagsId] ON [GameGameTag] ([TagsId]);

CREATE UNIQUE INDEX [IX_Gamers_AccountName] ON [Gamers] ([AccountName]);

CREATE UNIQUE INDEX [IX_Gamers_Url] ON [Gamers] ([Url]);

CREATE INDEX [IX_Games_LocalizationId] ON [Games] ([LocalizationId]);

CREATE UNIQUE INDEX [IX_Games_Name] ON [Games] ([Name]);

CREATE INDEX [IX_GamesCriticsReviews_CriticId] ON [GamesCriticsReviews] ([CriticId]);

CREATE INDEX [IX_GamesCriticsReviews_GameId] ON [GamesCriticsReviews] ([GameId]);

CREATE UNIQUE INDEX [IX_GamesCriticsReviews_Text] ON [GamesCriticsReviews] ([Text]);

CREATE UNIQUE INDEX [IX_GamesCriticsReviews_Url] ON [GamesCriticsReviews] ([Url]);

CREATE UNIQUE INDEX [IX_GamesDevelopers_Name] ON [GamesDevelopers] ([Name]);

CREATE UNIQUE INDEX [IX_GamesDevelopers_Url] ON [GamesDevelopers] ([Url]);

CREATE UNIQUE INDEX [IX_GamesGenres_Name] ON [GamesGenres] ([Name]);

CREATE UNIQUE INDEX [IX_GamesGenres_Url] ON [GamesGenres] ([Url]);

CREATE UNIQUE INDEX [IX_GamesLocalizations_Title] ON [GamesLocalizations] ([Title]);

CREATE UNIQUE INDEX [IX_GamesLocalizations_Url] ON [GamesLocalizations] ([Url]);

CREATE UNIQUE INDEX [IX_GamesPlatforms_Name] ON [GamesPlatforms] ([Name]);

CREATE UNIQUE INDEX [IX_GamesPlatforms_Url] ON [GamesPlatforms] ([Url]);

CREATE UNIQUE INDEX [IX_GamesPublishers_Name] ON [GamesPublishers] ([Name]);

CREATE UNIQUE INDEX [IX_GamesPublishers_Url] ON [GamesPublishers] ([Url]);

CREATE UNIQUE INDEX [IX_GamesTags_Title] ON [GamesTags] ([Title]);

CREATE UNIQUE INDEX [IX_GamesTags_Url] ON [GamesTags] ([Url]);

CREATE INDEX [IX_Trailers_GameId] ON [Trailers] ([GameId]);

CREATE UNIQUE INDEX [IX_Trailers_Url] ON [Trailers] ([Url]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241216081554_Initial', N'9.0.0');

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Games]') AND [c].[name] = N'ImageSource');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Games] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Games] ALTER COLUMN [ImageSource] nvarchar(511) NOT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241224100232_IncreaseImageSourceLength', N'9.0.0');

COMMIT;
GO

