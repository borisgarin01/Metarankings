﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20241214190443_ColumnDataTypeChanged")]
    partial class ColumnDataTypeChanged
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Collection", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Collections");
                });

            modelBuilder.Entity("Domain.CollectionItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("CollectionId")
                        .HasColumnType("bigint");

                    b.Property<string>("Href")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ImageSrc")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("CollectionId");

                    b.HasIndex("Href")
                        .IsUnique();

                    b.HasIndex("ImageSrc")
                        .IsUnique();

                    b.HasIndex("Title")
                        .IsUnique();

                    b.ToTable("CollectionItems");
                });

            modelBuilder.Entity("Domain.Critic", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(511)
                        .HasColumnType("nvarchar(511)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("Critics");
                });

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageSource")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("LocalizationId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("datetime2");

                    b.Property<float>("Score")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Domain.GameCriticReview", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("CriticId")
                        .HasColumnType("bigint");

                    b.Property<long>("GameId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ReviewDate")
                        .HasColumnType("datetime2");

                    b.Property<float>("Score")
                        .HasColumnType("real");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(511)
                        .HasColumnType("nvarchar(511)");

                    b.HasKey("Id");

                    b.HasIndex("CriticId");

                    b.HasIndex("GameId");

                    b.HasIndex("Text")
                        .IsUnique();

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("GamesCriticsReviews");
                });

            modelBuilder.Entity("Domain.GameDeveloper", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("GamesDevelopers");
                });

            modelBuilder.Entity("Domain.GameGamerReview", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("GameId")
                        .HasColumnType("bigint");

                    b.Property<long>("GamerId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ReviewDate")
                        .HasColumnType("datetime2");

                    b.Property<float>("Score")
                        .HasColumnType("real");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("GamerId");

                    b.HasIndex("Text")
                        .IsUnique();

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("GameGamerReview");
                });

            modelBuilder.Entity("Domain.GameGenre", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("GamesGenres");
                });

            modelBuilder.Entity("Domain.GameLocalization", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Title")
                        .IsUnique();

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("GamesLocalizations");
                });

            modelBuilder.Entity("Domain.GamePlatform", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("GamesPlatforms");
                });

            modelBuilder.Entity("Domain.GamePublisher", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("GamesPublishers");
                });

            modelBuilder.Entity("Domain.GameTag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Title")
                        .IsUnique();

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("GamesTags");
                });

            modelBuilder.Entity("Domain.Gamer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("AccountName")
                        .IsUnique();

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("Gamers");
                });

            modelBuilder.Entity("Domain.Trailer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long?>("GameId")
                        .HasColumnType("bigint");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("Trailers");
                });

            modelBuilder.Entity("GameGameDeveloper", b =>
                {
                    b.Property<long>("DevelopersId")
                        .HasColumnType("bigint");

                    b.Property<long>("GamesId")
                        .HasColumnType("bigint");

                    b.HasKey("DevelopersId", "GamesId");

                    b.HasIndex("GamesId");

                    b.ToTable("GameGameDeveloper");
                });

            modelBuilder.Entity("GameGameGenre", b =>
                {
                    b.Property<long>("GamesId")
                        .HasColumnType("bigint");

                    b.Property<long>("GenresId")
                        .HasColumnType("bigint");

                    b.HasKey("GamesId", "GenresId");

                    b.HasIndex("GenresId");

                    b.ToTable("GameGameGenre");
                });

            modelBuilder.Entity("GameGamePlatform", b =>
                {
                    b.Property<long>("GamesId")
                        .HasColumnType("bigint");

                    b.Property<long>("PlatformsId")
                        .HasColumnType("bigint");

                    b.HasKey("GamesId", "PlatformsId");

                    b.HasIndex("PlatformsId");

                    b.ToTable("GameGamePlatform");
                });

            modelBuilder.Entity("GameGamePublisher", b =>
                {
                    b.Property<long>("GamesId")
                        .HasColumnType("bigint");

                    b.Property<long>("PublishersId")
                        .HasColumnType("bigint");

                    b.HasKey("GamesId", "PublishersId");

                    b.HasIndex("PublishersId");

                    b.ToTable("GameGamePublisher");
                });

            modelBuilder.Entity("GameGameTag", b =>
                {
                    b.Property<long>("GamesId")
                        .HasColumnType("bigint");

                    b.Property<long>("TagsId")
                        .HasColumnType("bigint");

                    b.HasKey("GamesId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("GameGameTag");
                });

            modelBuilder.Entity("Domain.CollectionItem", b =>
                {
                    b.HasOne("Domain.Collection", "Collection")
                        .WithMany("CollectionItems")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Collection");
                });

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.HasOne("Domain.GameLocalization", "Localization")
                        .WithMany("Games")
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Localization");
                });

            modelBuilder.Entity("Domain.GameCriticReview", b =>
                {
                    b.HasOne("Domain.Critic", "Critic")
                        .WithMany("CriticReviews")
                        .HasForeignKey("CriticId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Game", "Game")
                        .WithMany("CriticsReviews")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Critic");

                    b.Navigation("Game");
                });

            modelBuilder.Entity("Domain.GameGamerReview", b =>
                {
                    b.HasOne("Domain.Game", "Game")
                        .WithMany("UsersReviews")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Gamer", "Gamer")
                        .WithMany("GameGamerReviews")
                        .HasForeignKey("GamerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Gamer");
                });

            modelBuilder.Entity("Domain.Trailer", b =>
                {
                    b.HasOne("Domain.Game", null)
                        .WithMany("Trailers")
                        .HasForeignKey("GameId");
                });

            modelBuilder.Entity("GameGameDeveloper", b =>
                {
                    b.HasOne("Domain.GameDeveloper", null)
                        .WithMany()
                        .HasForeignKey("DevelopersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Game", null)
                        .WithMany()
                        .HasForeignKey("GamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GameGameGenre", b =>
                {
                    b.HasOne("Domain.Game", null)
                        .WithMany()
                        .HasForeignKey("GamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.GameGenre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GameGamePlatform", b =>
                {
                    b.HasOne("Domain.Game", null)
                        .WithMany()
                        .HasForeignKey("GamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.GamePlatform", null)
                        .WithMany()
                        .HasForeignKey("PlatformsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GameGamePublisher", b =>
                {
                    b.HasOne("Domain.Game", null)
                        .WithMany()
                        .HasForeignKey("GamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.GamePublisher", null)
                        .WithMany()
                        .HasForeignKey("PublishersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GameGameTag", b =>
                {
                    b.HasOne("Domain.Game", null)
                        .WithMany()
                        .HasForeignKey("GamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.GameTag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Collection", b =>
                {
                    b.Navigation("CollectionItems");
                });

            modelBuilder.Entity("Domain.Critic", b =>
                {
                    b.Navigation("CriticReviews");
                });

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.Navigation("CriticsReviews");

                    b.Navigation("Trailers");

                    b.Navigation("UsersReviews");
                });

            modelBuilder.Entity("Domain.GameLocalization", b =>
                {
                    b.Navigation("Games");
                });

            modelBuilder.Entity("Domain.Gamer", b =>
                {
                    b.Navigation("GameGamerReviews");
                });
#pragma warning restore 612, 618
        }
    }
}